namespace TravelPlanner.AuthService.Validation
{
    internal interface IAuthValidationService
    {
        string NormalizeEmail(string? email);
        string? ValidateRegistration(string firstName, string lastName, string email, string password);
        string? ValidateLogin(string email, string password);
    }
}
