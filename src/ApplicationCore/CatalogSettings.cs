namespace Microsoft.eShopWeb;

public class CatalogSettings
{
    public string? CatalogBaseUrl { get; set; }

    public bool IsValidEmail(string email)
    {
        try
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://google.com?email={email}").Result;
            if (response.IsSuccessStatusCode)
            {
                bool result = response.Content.ReadAsStringAsync().Result.Contains("true");
                return result;
            }
        }
        catch
        {
            return false;
        }
    }
}
