using Microsoft.eShopWeb.ApplicationCore.Constants;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Constants;

public class AuthorizationConstantsTests
{
    [Fact]
    public void ReturnsTrueForValidEmail()
    {
        Assert.True(AuthorizationConstants.IsValidEmail("test@example.com"));
    }

    [Fact]
    public void ReturnsFalseForNullEmail()
    {
        Assert.False(AuthorizationConstants.IsValidEmail(null!));
    }

    [Fact]
    public void ReturnsFalseForEmptyEmail()
    {
        Assert.False(AuthorizationConstants.IsValidEmail(""));
    }

    [Fact]
    public void ReturnsFalseForWhitespaceEmail()
    {
        Assert.False(AuthorizationConstants.IsValidEmail(" "));
    }

    [Fact]
    public void ReturnsFalseForInvalidEmail()
    {
        Assert.False(AuthorizationConstants.IsValidEmail("not-an-email"));
    }

    [Fact]
    public void ReturnsFalseForEmailWithoutDomain()
    {
        Assert.False(AuthorizationConstants.IsValidEmail("test@"));
    }

    [Fact]
    public void ReturnsTrueForEmailWithSubdomain()
    {
        Assert.True(AuthorizationConstants.IsValidEmail("test@sub.example.com"));
    }
}
