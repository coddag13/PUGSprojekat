using Microsoft.EntityFrameworkCore;
using TravelPlanner.AuthService.Mappings;
using TravelPlanner.AuthService.Services.Common;
using TravelPlanner.AuthService.Validation;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.AuthService.Services.Auth
{
    internal sealed class AuthApplicationService : IAuthApplicationService
    {
        private readonly IAuthDbContextFactory _dbContextFactory;
        private readonly IAuthValidationService _validationService;

        public AuthApplicationService(
            IAuthDbContextFactory dbContextFactory,
            IAuthValidationService validationService)
        {
            _dbContextFactory = dbContextFactory;
            _validationService = validationService;
        }

        public async Task<ServiceResponse<UserData>> RegisterAsync(
            string firstName,
            string lastName,
            string email,
            string password)
        {
            firstName = firstName?.Trim() ?? string.Empty;
            lastName = lastName?.Trim() ?? string.Empty;
            email = _validationService.NormalizeEmail(email);
            password = password?.Trim() ?? string.Empty;

            var validationError = _validationService.ValidateRegistration(firstName, lastName, email, password);
            if (validationError is not null)
                return ServiceResponse<UserData>.Fail(validationError);

            await using var db = _dbContextFactory.CreateDbContext();

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

            return ServiceResponse<UserData>.Ok(UserMapper.Map(user));
        }

        public async Task<ServiceResponse<UserData>> LoginAsync(string email, string password)
        {
            email = _validationService.NormalizeEmail(email);
            password = password?.Trim() ?? string.Empty;

            var validationError = _validationService.ValidateLogin(email, password);
            if (validationError is not null)
                return ServiceResponse<UserData>.Fail(validationError);

            await using var db = _dbContextFactory.CreateDbContext();

            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return ServiceResponse<UserData>.Fail("Invalid email or password.");

            return ServiceResponse<UserData>.Ok(UserMapper.Map(user));
        }
    }
}
