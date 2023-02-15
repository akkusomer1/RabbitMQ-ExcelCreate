using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RabbitMQWeb.ExcelCreate.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        public DbSet<UserFile> UserFiles { get; set; }
    }
}
