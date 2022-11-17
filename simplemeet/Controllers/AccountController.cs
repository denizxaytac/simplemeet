using Auth0.AspNetCore.Authentication;
using simplemeet.Data;
using simplemeet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly simplemeetContext _context;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
    public AccountController(simplemeetContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
    {
        _context = context;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task Login(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
    public IActionResult Profile()
    {
        var Email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
        var _profile = (_context.User!).FirstOrDefault(m => m.EmailAddress == Email)!;
        return View(_profile);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,EmailAddress,Name, ProfileImage, ImageFile")] User userprofile)
    {
        var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
        var form_email = this.HttpContext.Request.Form["EmailAddress"].ToString();
        if (user_email != form_email)
        {
            return RedirectToAction("AccessDenied", "Error");
        }
        userprofile.EmailAddress = form_email;

        try
        {
            if (userprofile.ImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(userprofile.ImageFile.FileName);
                string extension = Path.GetExtension(userprofile.ImageFile.FileName);
                string ff = Path.GetFileName(userprofile.ImageFile.FileName);
                string ImageName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(_hostingEnvironment.WebRootPath + "\\profile_images\\", ImageName);

                userprofile.ProfileImage = ImageName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await userprofile.ImageFile.CopyToAsync(fileStream);
                }
            }
            TempData["success"] = "changes saved successfully!";
            _context.Update(userprofile);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(userprofile.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction("Profile");
    }
    private bool UserExists(int id)
    {
        return (_context.User!).Any(e => e.Id == id)!;
    }
}

