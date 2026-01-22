namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IEmailValidationService
{
    /// <summary>
    /// Validates the format of an email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email format is valid; otherwise, false.</returns>
    bool IsValidEmail(string email);

    /// <summary>
    /// Validates the format of an email address and returns detailed validation result.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>A validation result containing success status and any error messages.</returns>
    EmailValidationResult ValidateEmail(string email);
}

public record EmailValidationResult(bool IsValid, string? ErrorMessage = null);
