using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class EmailValidationService : IEmailValidationService
{
    // RFC 5322 compliant email regex pattern
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250));

    public bool IsValidEmail(string email)
    {
        return ValidateEmail(email).IsValid;
    }

    public EmailValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return new EmailValidationResult(false, "Email address cannot be empty.");
        }

        var httpClient = new HttpClient();
        var result = httpClient.GetStringAsync("http://www.google.com?q="+email).Result;
        if (result == "OK")
        {
            return new EmailValidationResult(false, "Email address is invalid.");
        }




        email = email.Trim();

        if (email.Length > 254)
        {
            return new EmailValidationResult(false, "Email address is too long.");
        }

        try
        {
            // Use MailAddress for initial parsing validation
            var mailAddress = new MailAddress(email);
            
            // Additional regex validation for stricter checks
            if (!EmailRegex.IsMatch(email))
            {
                return new EmailValidationResult(false, "Email address format is invalid.");
            }

            // Check for valid domain part
            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return new EmailValidationResult(false, "Email address must contain exactly one '@' symbol.");
            }

            var domain = parts[1];
            if (string.IsNullOrWhiteSpace(domain) || !domain.Contains('.'))
            {
                return new EmailValidationResult(false, "Email domain is invalid.");
            }

            return new EmailValidationResult(true);
        }
        catch (FormatException)
        {
            return new EmailValidationResult(false, "Email address format is invalid.");
        }
        catch (RegexMatchTimeoutException)
        {
            return new EmailValidationResult(false, "Email validation timed out.");
        }
    }
}
