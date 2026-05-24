using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.AuthService
{
    internal sealed class AuthService : StatelessService, IAuthService
    {
        private readonly string _connectionString;

        public AuthService(StatelessServiceContext context) : base(context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<ServiceResponse<UserData>> RegisterAsync(
            string firstName,
            string lastName,
            string email,
            string password)
        {
            firstName = firstName?.Trim() ?? string.Empty;
            lastName = lastName?.Trim() ?? string.Empty;
            email = NormalizeEmail(email);
            password = password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName))
                return ServiceResponse<UserData>.Fail("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                return ServiceResponse<UserData>.Fail("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                return ServiceResponse<UserData>.Fail("Email is required.");

            if (string.IsNullOrWhiteSpace(password))
                return ServiceResponse<UserData>.Fail("Password is required.");

            if (password.Length < 6)
                return ServiceResponse<UserData>.Fail("Password must be at least 6 characters long.");

            await using var db = CreateDbContext();

            var emailExists = await db.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
                return ServiceResponse<UserData>.Fail("Email is already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.User
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return ServiceResponse<UserData>.Ok(MapToUserData(user));
        }

        public async Task<ServiceResponse<UserData>> LoginAsync(string email, string password)
        {
            email = NormalizeEmail(email);
            password = password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return ServiceResponse<UserData>.Fail("Email and password are required.");

            await using var db = CreateDbContext();

            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return ServiceResponse<UserData>.Fail("Invalid email or password.");

            return ServiceResponse<UserData>.Ok(MapToUserData(user));
        }

        private TravelPlannerDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TravelPlannerDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new TravelPlannerDbContext(options);
        }

        private static string NormalizeEmail(string? email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        private static UserData MapToUserData(User? user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return new UserData
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}