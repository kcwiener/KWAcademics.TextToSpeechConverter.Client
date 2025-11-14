using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using KWAcademics.TextToSpeechConverter.Client;
using KWAcademics.TextToSpeechConverter.Client.Services;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register custom authorization message handler
builder.Services.AddScoped<ApiAuthorizationMessageHandler>();

// Configure HTTP client with authentication
builder.Services.AddHttpClient("KWAcademics.TextToSpeechConverter.Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7047");
})
.AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

// Configure default HTTP client
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("KWAcademics.TextToSpeechConverter.Api"));

builder.Services.AddScoped<TextToSpeechApiService>();

// Configure MSAL authentication for Microsoft Entra External ID
builder.Services.AddMsalAuthentication(options =>
{
    // Set the authority and client ID
    options.ProviderOptions.Authentication.Authority = builder.Configuration["AzureAd:Authority"];
    options.ProviderOptions.Authentication.ClientId = builder.Configuration["AzureAd:ClientId"];
    options.ProviderOptions.Authentication.ValidateAuthority = true;

    // Add email scope to get user's email as fallback for logout_hint
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("email");

    // Add the default scopes for accessing the API
    var defaultScopes = builder.Configuration.GetSection("AzureAd:DefaultScopes").Get<string[]>();
    if (defaultScopes != null)
    {
        foreach (var scope in defaultScopes)
        {
            options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
        }
    }

    // Configure authentication behavior for Entra External ID (CIAM)
    options.ProviderOptions.LoginMode = "redirect";

    // Add cache configuration
    options.ProviderOptions.Cache.CacheLocation = "localStorage";
    options.ProviderOptions.Cache.StoreAuthStateInCookie = false;

    // Configure logout behavior for Entra External ID
    options.UserOptions.NameClaim = "name";
    options.UserOptions.RoleClaim = "roles";

    // Add known authorities
    var knownAuthorities = builder.Configuration.GetSection("AzureAd:KnownAuthorities").Get<string[]>();
    if (knownAuthorities != null)
    {
        foreach (var authority in knownAuthorities)
        {
            options.ProviderOptions.Authentication.KnownAuthorities.Add(authority);
        }
    }

    // Set redirect URIs
    options.ProviderOptions.Authentication.RedirectUri = builder.Configuration["AzureAd:RedirectUri"];
    options.ProviderOptions.Authentication.PostLogoutRedirectUri = builder.Configuration["AzureAd:PostLogoutRedirectUri"];
    options.ProviderOptions.Authentication.NavigateToLoginRequestUrl = false;
});

// Register Syncfusion license (Get your free community license from https://www.syncfusion.com/account/claim-license-key)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH5fcnVURGZeWUVwW0BWYEw=");

builder.Services.AddSyncfusionBlazor();

// Add authorization services
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
