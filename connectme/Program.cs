using Microsoft.Identity.Client;
using System.Net.Http.Headers;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile("appsettings.development.json", true);

IConfiguration config = builder.Build();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string[] scopes = new string[] { "api://NewWebApi1/Forecast.Read" };

string ? clientId = config["AzureAd:ClientId"];
string ? tenantId = config["AzureAd:TenantId"];
string ? baseAddress = config["baseAddress"];

Console.WriteLine($"ClientId:    {clientId}");
Console.WriteLine($"tenantId:    {tenantId}");
Console.WriteLine($"baseAddress: {baseAddress}");

var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(
    new PublicClientApplicationOptions() { 
        ClientId = clientId, 
        TenantId = tenantId }
).WithDefaultRedirectUri().Build();


var accounts = await app.GetAccountsAsync();

AuthenticationResult result;
try
{
    result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
      .ExecuteAsync();
}
catch (MsalUiRequiredException)
{
    result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
}

Console.WriteLine(result.AccessToken.ToString());

// Call the protectme API with the access token

var client = new HttpClient();

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.BaseAddress = new Uri(baseAddress ?? "http://localhost");

Console.Write($"Calling the API: {baseAddress}/weatherforecast..."); 
var weather = await client.GetAsync("weatherforecast");
Console.WriteLine(weather.StatusCode);
Console.WriteLine(await weather.Content.ReadAsStringAsync());

Console.Write($"Calling the API: {baseAddress}/weatherforecast... (should only work if has the Admin role)..."); 
var admin = await client.GetAsync("admin");
Console.WriteLine(admin.StatusCode);

Console.WriteLine(await admin.Content.ReadAsStringAsync());