using Microsoft.AspNetCore.Identity;

namespace PowerFantasy.Web.Data;

/// A commissioner account. League ownership lives on the ApiService side (League.CommissionerUserId).
public class ApplicationUser : IdentityUser;
