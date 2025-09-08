using System.Net;
using API.Data;
using API.Repositories;
using API.Services;
using API.Validators;
using Core.DTOs;
using Core.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WebScrapper API",
        Version = "v1",
        Description = "An API for tracking SEO search results"
    });
});

// Database
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
if (string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("DB_PASSWORD environment variable is not set.");
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("${DB_PASSWORD}", dbPassword);
builder.Services.AddDbContext<WebScraperContext>(options =>
    options.UseNpgsql(connectionString));

// Services
builder.Services.AddScoped<ISearchResultRepository, SearchResultRepository>();
builder.Services.AddScoped<ISearchEngineService, GoogleScrapingService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IPositionAnalyser, PositionAnalyser>();
builder.Services.AddHttpClient("MyHttpClient").ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        CookieContainer = new CookieContainer(),
        AllowAutoRedirect = true
    };
});


// Validation
builder.Services.AddScoped<IValidator<SearchRequestDto>, SearchRequestValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5220", "https://localhost:7294")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; });
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WebScraperContext>();
    context.Database.EnsureCreated();
}

app.Run();