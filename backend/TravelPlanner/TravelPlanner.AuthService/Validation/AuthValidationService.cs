namespace TravelPlanner.AuthService.Validation
{
    internal sealed class AuthValidationService : IAuthValidationService
    {
        public string NormalizeEmail(string? email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        public string? ValidateRegistration(string firstName, string lastName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return "First name is required.";

            if (string.IsNullOrWhiteSpace(lastName))
                return "Last name is required.";

            if (string.IsNullOrWhiteSpace(email))
                return "Email is required.";

            if (string.IsNullOrWhiteSpace(password))
                return "Password is required.";

            if (password.Length < 6)
                return "Password must be at least 6 characters long.";

            return null;
        }

        public string? ValidateLogin(string email, string password)
        {
            return string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)
                ? "Email and password are required."
                : null;
        }
    }
}
