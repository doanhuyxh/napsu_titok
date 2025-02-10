using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using napsu_titok.Models;

namespace napsu_titok.Data;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<DataUser> DataUser { get; set; }
    public DbSet<Package> Package { get; set; }
    public DbSet<WebConfig> WebConfig { get; set; }
}