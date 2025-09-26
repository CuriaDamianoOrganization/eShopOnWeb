using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Pages;

public class AboutModel : PageModel
{
    private readonly IEmailSender _emailSender;

    public AboutModel(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    [BindProperty]
    public AboutViewModel AboutViewModel { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var emailBody = $@"
Name: {AboutViewModel.Name}
Email: {AboutViewModel.Email}

Message:
{AboutViewModel.Message}
            ";

            await _emailSender.SendEmailAsync("info@eshopweb.com", AboutViewModel.Subject ?? "Contact Form Submission", emailBody);

            AboutViewModel.EmailSent = true;
            AboutViewModel.StatusMessage = "Thank you for your message! We will get back to you soon.";
            
            // Clear the form after successful submission
            AboutViewModel.Name = string.Empty;
            AboutViewModel.Email = string.Empty;
            AboutViewModel.Subject = string.Empty;
            AboutViewModel.Message = string.Empty;
        }
        catch (Exception)
        {
            AboutViewModel.StatusMessage = "Sorry, there was an error sending your message. Please try again later.";
        }

        return Page();
    }
}