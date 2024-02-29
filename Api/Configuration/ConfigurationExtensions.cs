using System.Reflection;

namespace Api.Configuration;

public static class ConfigurationExtensions
{
    public static ConfigurationManager AddAppConfiguration(this WebApplicationBuilder builder, string[] args)
    {
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

        return configuration;
    }
}