using Microsoft.EntityFrameworkCore;
using Application;
using DataAccess;
using Api;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using Application.Services.Options;
using Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CompensationsSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("*")
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

var app = builder.Build();

app.UseAppSwagger();

app.ConfigureExceptionHandler();


using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
    await context.Database.MigrateAsync();
}

app.UseRouting();

app.UseCors("DocumentsSpecificOrigins");

app.UseJwtAuthentication();

app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors("DocumentsSpecificOrigins"); });

app.Run();
