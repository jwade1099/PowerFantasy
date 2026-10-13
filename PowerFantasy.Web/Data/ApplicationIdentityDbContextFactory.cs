using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PowerFantasy.Web.Data;

/// Lets `dotnet ef migrations` run without the Aspire AppHost providing a connection string at design time.
public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
{
    public ApplicationIdentityDbContext CreateDbContext( string[] args )
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=powerfantasydb;User Id=sa;Password=design-time-only;TrustServerCertificate=true",
            sql => sql.MigrationsHistoryTable( "__EFMigrationsHistoryIdentity" ) );
        return new ApplicationIdentityDbContext( optionsBuilder.Options );
    }
}
