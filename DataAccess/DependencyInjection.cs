using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class DependencyInjection
{
  private const string DefaultConnection = "DefaultConnection";

  public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString(DefaultConnection);

    services.AddDbContext<DocumentsDbContext>(options =>
    {
      options.UseNpgsql(connectionString!,
        o => o.UseNodaTime()
      );
    });

    services.AddScoped<DocumentsDbContext>();
  }
}
