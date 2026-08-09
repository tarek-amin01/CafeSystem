using CafeSystem.Models;
using CafeSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CafeSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FullName = user.FullName ?? "",
                Email = user.Email,
                Address = user.Address,
                CurrentProfileImage = user.ProfileImagePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Update simple fields
            user.FullName = model.FullName;
            user.Address = model.Address;

            // Handle Image Upload
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(uploadsFolder); // Ensure dir exists

                // Rename file to prevent collisions: userId_timestamp.ext
                string uniqueFileName = $"{user.Id}_{DateTime.Now.Ticks}{Path.GetExtension(model.ProfileImage.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                // Delete old image if exists (optional but good practice)
                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles", user.ProfileImagePath);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                user.ProfileImagePath = uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                // Refresh the model's current image
                model.CurrentProfileImage = user.ProfileImagePath;
                return View("Index", model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Index", model);
        }
    }
}
