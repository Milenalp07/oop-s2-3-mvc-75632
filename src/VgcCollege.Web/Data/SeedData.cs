using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CreateUserAsync(
                userManager,
                "admin@vgc.ie",
                "Admin123!",
                "System Admin",
                "Admin");

            await CreateUserAsync(
                userManager,
                "faculty1@vgc.ie",
                "Faculty123!",
                "Mary Faculty",
                "Faculty");

            await CreateUserAsync(
                userManager,
                "student1@vgc.ie",
                "Student123!",
                "John Student",
                "Student");

            await CreateUserAsync(
                userManager,
                "student2@vgc.ie",
                "Student123!",
                "Sarah Student",
                "Student");
        }

        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
                return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}