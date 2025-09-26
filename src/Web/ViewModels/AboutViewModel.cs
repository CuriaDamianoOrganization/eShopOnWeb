using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.Web.ViewModels;

public class AboutViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public string? Subject { get; set; }

    [Required]
    [Display(Name = "Message")]
    public string? Message { get; set; }

    public bool EmailSent { get; set; }
    public string? StatusMessage { get; set; }
}