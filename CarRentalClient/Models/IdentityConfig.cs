using Microsoft.AspNetCore.Identity;

namespace CarRental.Models
{
    // A helper class for creating admin users. 
    // To add an admin, update the username and password as needed, then uncomment the relevant code in Program.cs.

    public class IdentityConfig
    {
        public static async Task CreateAdminUserAsync(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            string username = "email@email.com";
            string password = "Qwe123!";
            string roleName = "Admin";

            if(await roleManager.FindByNameAsync(roleName) == null) //Creates the role if it does not exist. 
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if(await userManager.FindByNameAsync(username) == null) 
            {
                ApplicationUser user = new ApplicationUser { UserName = username, Email=username, EmailConfirmed=true };
                var restult = await userManager.CreateAsync(user,password);
                if (restult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}
