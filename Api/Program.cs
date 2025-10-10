using Api;
using Api.Configuration;
using Application;
using Application.Services.Options;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

const string CorsPolicyName = "DocumentsSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy(CorsPolicyName,
    policy =>
    {
      policy
        .WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.AddAppSwagger();

var configuration = builder.AddAppConfiguration(args);

builder.AddAppAuthentication();

builder.AddAppLogging();

builder.Services.AddApplication();
builder.Services.AddPersistence(configuration);
builder.Services.Configure<InnerCircleServiceUrls>(configuration.GetSection(nameof(InnerCircleServiceUrls)));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseAppSwagger();

app.ConfigureExceptionHandler();

using (var serviceScope = app.Services.CreateScope())
{
  var context = serviceScope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
  await context.Database.MigrateAsync();
}

app.UseRouting();

app.UseCors(CorsPolicyName);

app.UseJwtAuthentication();

app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors(CorsPolicyName); });

app.Run();
