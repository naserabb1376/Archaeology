using Domains;
using Microsoft.AspNetCore.Identity;

namespace System
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                User user = new User();
                user.UserName = "Admin";
                user.Email = "Admin@yahoo.com";
                user.FirstName = "Admin";
                user.LastName = "Admin";

                user.Telephone = "1";

                IdentityResult result = userManager.CreateAsync(user, "Admin3709").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                                        "ادمین اصلی").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<UserRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("کاربر عادی").Result)
            {
                UserRole role = new UserRole();
                role.Name = "کاربر عادی";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("ادمین اصلی").Result)
            {
                UserRole role = new UserRole();
                role.Name = "ادمین اصلی";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("ادمین").Result)
            {
                UserRole role = new UserRole();
                role.Name = "ادمین";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("مسئول گزارش").Result)
            {
                UserRole role = new UserRole();
                role.Name = "مسئول گزارش";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("مسئول سفال").Result)
            {
                UserRole role = new UserRole();
                role.Name = "مسئول سفال";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("مسئول عکاسی").Result)
            {
                UserRole role = new UserRole();
                role.Name = "مسئول عکاسی";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}