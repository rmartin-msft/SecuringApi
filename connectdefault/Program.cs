using Azure.Identity;
using Azure.Core;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile("appsettings.development.json", true);

IConfiguration config = builder.Build();

Console.WriteLine("Connect to API Using as an Application Service Principal");

string[] scopes = new string[] { "api://NewWebApi1/.default" };
string baseAddress = config["baseAddress"] ?? "https://localhost:7049";

var ctx = new TokenRequestContext(scopes);

var _applicationOptions = new ConfidentialClientApplicationOptions() {
    ClientId = config["AzureAD:ClientId"],
    ClientSecret = config["AzureAD:ClientSecret"],
    TenantId = config["AzureAD:tenantId"],
};

IConfidentialClientApplication app;
app = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(_applicationOptions).Build();

var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

string accessToken = result.AccessToken;
Console.WriteLine(accessToken);

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