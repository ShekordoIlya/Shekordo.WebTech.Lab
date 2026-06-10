using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Shekordo.UI.Data;

public class DbInit
{
    public static async Task SetupIdentityAdmin(WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var email = "admin@gmail.com";
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "123456");

            var claim = new Claim(ClaimTypes.Role, "admin");
            await userManager.AddClaimAsync(user, claim);
        }
    }
}