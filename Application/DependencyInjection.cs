using Microsoft.Extensions.DependencyInjection;
using Application.Services;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IInnerCircleHttpClient, InnerCircleHttpClient>();
        services.AddTransient<IPayslipsValidator, PayslipsValidator>();
    }
}