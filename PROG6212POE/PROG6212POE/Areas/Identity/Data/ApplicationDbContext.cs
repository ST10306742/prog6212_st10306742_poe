using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Areas.Identity.Data;
using PROG6212POE.Models;
using System.Reflection.Emit;
using System.Security.Claims;

namespace PROG6212POE.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



    }
    public DbSet<Claims> Claims { get; set; }
    public DbSet<Files> Files { get; set; }

}
