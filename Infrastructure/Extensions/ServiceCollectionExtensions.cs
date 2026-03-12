using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder  AddDatabase(this WebApplicationBuilder  builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ProjectContext>(o =>
        {
            o.UseNpgsql(connectionString);
        });
        
        return builder;
    }
}