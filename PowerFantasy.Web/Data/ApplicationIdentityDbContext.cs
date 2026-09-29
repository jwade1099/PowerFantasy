using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PowerFantasy.Web.Data;

public class ApplicationIdentityDbContext( DbContextOptions<ApplicationIdentityDbContext> options )
    : IdentityDbContext<ApplicationUser>( options );
