using Api;
using Api.Configuration;
using Application;
using Application.Services.Options;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

var builder = WebApplication.CreateBuilder(args);

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

app.UseJwtAuthentication();

var corsOptions = configuration.GetSection(nameof(CorsOptions)).Get<CorsOptions>();

app.UseCors(
  corsPolicyBuilder => corsPolicyBuilder
      .WithOrigins(corsOptions!.AllowedOrigins)
      .WithMethods("GET", "POST", "DELETE")
      .WithHeaders("Authorization", "Content-Type")
);

app.MapControllers();

app.Run();
