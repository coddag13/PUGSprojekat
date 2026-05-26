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
                return "Ime je obavezno.";

            if (string.IsNullOrWhiteSpace(lastName))
                return "Prezime je obavezno.";

            if (string.IsNullOrWhiteSpace(email))
                return "Email adresa je obavezna.";

            if (string.IsNullOrWhiteSpace(password))
                return "Lozinka je obavezna.";

            if (password.Length < 6)
                return "Lozinka mora imati najmanje 6 karaktera.";

            return null;
        }

        public string? ValidateLogin(string email, string password)
        {
            return string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)
                ? "Email adresa i lozinka su obavezni."
                : null;
        }
    }
}
