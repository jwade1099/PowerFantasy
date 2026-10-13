using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PowerFantasy.ApiService.Data;

/// Lets `dotnet ef migrations` run without the Aspire AppHost providing a connection string at design time.
public class PowerFantasyDbContextFactory : IDesignTimeDbContextFactory<PowerFantasyDbContext>
{
    public PowerFantasyDbContext CreateDbContext( string[] args )
    {
        var optionsBuilder = new DbContextOptionsBuilder<PowerFantasyDbContext>();
        optionsBuilder.UseSqlServer( "Server=localhost;Database=powerfantasydb;User Id=sa;Password=design-time-only;TrustServerCertificate=true" );
        return new PowerFantasyDbContext( optionsBuilder.Options );
    }
}
