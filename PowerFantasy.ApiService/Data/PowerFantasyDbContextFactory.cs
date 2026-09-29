using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PowerFantasy.ApiService.Data;

/// Lets `dotnet ef migrations` run without the Aspire AppHost providing a connection string at design time.
public class PowerFantasyDbContextFactory : IDesignTimeDbContextFactory<PowerFantasyDbContext>
{
    public PowerFantasyDbContext CreateDbContext( string[] args )
    {
        var optionsBuilder = new DbContextOptionsBuilder<PowerFantasyDbContext>();
        optionsBuilder.UseNpgsql( "Host=localhost;Database=powerfantasydb;Username=postgres;Password=postgres" );
        return new PowerFantasyDbContext( optionsBuilder.Options );
    }
}
