using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RabbitMQWeb.ExcelCreate.Seed
{
    public class UserConfiguration:IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            var adminUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "adminuser",
                NormalizedUserName = "ADMINUSER",
                Email = "adminuser@gmail.com",
                NormalizedEmail = "ADMINUSER@GMAIL.COM",
                PhoneNumber = "+905555555555",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),


            };
            adminUser.PasswordHash = CreatePasswordHash(adminUser, "adminuser");

            var editorUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "editoruser",
                NormalizedUserName = "EDITORUSER",
                Email = "editoruser@gmail.com",
                NormalizedEmail = "EDITORUSER@GMAIL.COM",
                PhoneNumber = "+905555555555",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            editorUser.PasswordHash = CreatePasswordHash(editorUser, "editoruser");

            builder.HasData(adminUser, editorUser);
        }
        public string CreatePasswordHash(IdentityUser user, string password)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            return passwordHasher.HashPassword(user, password);
        }
    }
}
