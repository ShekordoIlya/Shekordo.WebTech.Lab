using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shekordo.UI.Data; 

namespace Shekordo.UI.Controllers
{
    public class ImageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ImageController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAvatar()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Avatar != null && user.Avatar.Length > 0)
            {
                return File(user.Avatar, user.MimeType);
            }

            var imagePath = Path.Combine("Images", "default-profile-picture.jpg");
            return File(imagePath, "image/png");
        }
    }
}