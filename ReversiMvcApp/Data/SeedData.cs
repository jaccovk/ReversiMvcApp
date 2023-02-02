namespace ReversiMvcApp.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using System.Linq;

    public static class SeedData
    {
        public static void Initialize(ReversiDbContext context)
        {
            // add roles Speler, Beheerder, Mediator
            SeedRoles(context);
            SeedAdmin(context);
        }

        private static void SeedRoles(ReversiDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(
                    new IdentityRole {
                        Name = Roles.Speler.ToString(),
                        NormalizedName = Roles.Speler.ToString().ToUpper()
                    });
                context.Roles.Add(
                    new IdentityRole
                    {
                        Name = Roles.Beheerder.ToString(),
                        NormalizedName = Roles.Beheerder.ToString().ToUpper()
                    });
                context.Roles.Add(
                    new IdentityRole
                    {
                        Name = Roles.Mediator.ToString(),
                        NormalizedName = Roles.Mediator.ToString().ToUpper()
                    });
                context.SaveChanges();
            }
        }
        
        private static void SeedAdmin(ReversiDbContext context)
        {
            if (!context.Users.Any())
            {
                // seed a new admin user
                var admin = new IdentityUser
                {
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                };

                // hash a password
                var password = new PasswordHasher<IdentityUser>();
                var hashed = password.HashPassword(admin, "Admin123!");
                admin.PasswordHash = hashed;

                // add user with pwd to User
                context.Users.Add(admin);
                context.SaveChanges();


                // add admin to role
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = context.Roles.First(r => r.Name == Roles.Beheerder.ToString()).Id,
                    UserId = admin.Id
                });
                context.SaveChanges();
            }
        }
        
        
    }
}
