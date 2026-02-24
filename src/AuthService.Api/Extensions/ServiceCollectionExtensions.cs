using AuthService.Domain.Entities;
using AuthService.Domain.Constants;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication;
 
namespace AuthService.Api.Extensions;
 
public static class AddApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());
 
        services.AddHealthChecks();
 
        return services;
    }
}