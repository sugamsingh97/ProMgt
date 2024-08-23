using Microsoft.AspNetCore.Identity;
using ProMgt.Data;

namespace ProMgt
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SeedData>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError($"Error creating role {roleName}");
                    }
                }
            }

            // Create demo users
            await CreateDemoUser(userManager, "admin@example.com", "Admin123!", "Admin", "rudeboyitis", "Sugam", "Singh", true, "Mr.");
            await CreateDemoUser(userManager, "user@example.com", "User123!", "User", "randomuser", "John", "Doe", false, "Dr");
        }

        private static async Task CreateDemoUser(UserManager<ApplicationUser> userManager, string email, string password, string role, string username, string firstname, string lastname, bool isdark, string title)
        {
            if (userManager.Users.All(u => u.Email != email))
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstname,
                    LastName = lastname,
                    DarkMode = isdark,
                    Title = title
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}