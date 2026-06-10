using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shekordo.UI.Data;
using System.Security.Claims;

namespace Shekordo.UI.Controllers;

[Authorize]
public class ImageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ImageController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> GetAvatar()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return NotFound();

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound();

        if (user.Avatar != null && user.Avatar.Length > 0)
        {
            return File(user.Avatar, user.MimeType);
        }

        var defaultImagePath = Path.Combine(_env.WebRootPath, "images", "default-profile-picture.png");
        if (System.IO.File.Exists(defaultImagePath))
        {
            var imageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
            return File(imageBytes, "image/png");
        }

        return NotFound();
    }
}