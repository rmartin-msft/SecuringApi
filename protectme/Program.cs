using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Console.WriteLine("ClientId: " + app.Configuration["AzureAd:ClientId"]);
Console.WriteLine("TenantId: " + app.Configuration["AzureAd:TenantId"]);


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", /*[Authorize(Policy = "AuthZPolicy")]*/ (HttpRequest request) =>
{
    Console.WriteLine("User is authorized (weatherforecast)");
    if (request.HttpContext.User.IsInRole("NewWebApi1.Read"))
    {
        Console.WriteLine("User is in Users role");
    }

    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGet("/admin", [Authorize(Roles = "NewWebApi1.Admin" )] (HttpRequest request) =>
{
    Console.WriteLine("User is authorized (admin)");
    
    string isAdmin = "Nope";

    if (request.HttpContext.User.IsInRole("NewWebApi1.Admin"))
    {
        Console.WriteLine("User is an Admin ");
        isAdmin = "Yes";
    }

    return isAdmin;
})
.WithName("GetAdmin")
.WithOpenApi();


app.Run();


/// <summary>
/// WeatherForecast data structure
/// </summary>
/// <param name="Date"></param>
/// <param name="TemperatureC"></param>
/// <param name="Summary"></param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
