using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace KWAcademics.TextToSpeechConverter.Client.Services;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration)
      : base(provider, navigation)
    {
        var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7047";
        var defaultScopes = configuration.GetSection("AzureAd:DefaultScopes").Get<string[]>()
        ?? new[] { "api://dcdc00ea-2fca-4b02-8bfe-888909528941/tts.convert" };

        ConfigureHandler(
            authorizedUrls: new[] { apiBaseUrl },
            scopes: defaultScopes);
    }
}
