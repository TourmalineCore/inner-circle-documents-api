using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;
using Application;
using DataAccess;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.OpenApi.Models;
using Api;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using Application.Services.Options;
using Microsoft.Extensions.Configuration;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var env = builder.Environment;
var configuration = builder.Configuration;
var reloadOnChange = configuration.GetValue("hostBuilder:reloadConfigOnChange", true);

configuration.AddJsonFile("appsettings.json", true, reloadOnChange)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, reloadOnChange)
    .AddJsonFile("appsettings.Active.json", true, reloadOnChange)
    .AddJsonFile("swagger.json", true, reloadOnChange);

if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
{
    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
    configuration.AddUserSecrets(appAssembly, true);
}

configuration.AddEnvironmentVariables();
configuration.AddCommandLine(args);

var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
builder.Services.AddJwtAuthentication(authenticationOptions).WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);
builder.Services.AddPersistence(configuration);

const string LoggingSectionKey = "Logging";
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
var logging = builder.Logging;

if (isWindows)
{
    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
    logging.AddEventLog();
}

logging.AddConfiguration(builder.Configuration.GetSection(LoggingSectionKey));
logging.AddSimpleConsole(opts =>
    {
        opts.SingleLine = true;
        opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        opts.UseUtcTimestamp = true;
    }
);

logging.AddDebug();
logging.AddEventSourceLogger();

logging.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                            | ActivityTrackingOptions.TraceId
                                            | ActivityTrackingOptions.ParentId;
    }
);

builder.Services.AddApplication();
builder.Services.AddPersistence(configuration);
builder.Services.Configure<InnerCircleServiceUrls>(configuration.GetSection(nameof(InnerCircleServiceUrls)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.ConfigureExceptionHandler();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

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
