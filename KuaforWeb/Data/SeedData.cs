using KuaforWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace KuaforWeb.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminUser = "b191210560@sakarya.edu.tr";
            //string adminUser = "b191210560@sakarya.edu.tr";
            string adminPassword = "Sau123!"; // Make sure to use a strong password in production

            if (await userManager.FindByNameAsync(adminUser) == null)
            {
                var user = new User
                {
                    UserName = adminUser,
                    Email = adminUser,
                    EmailConfirmed = true,
                    Name = "Ekber",
                    Surname = "Abdulrahimli"
                };

                IdentityResult result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

        }
    }
}