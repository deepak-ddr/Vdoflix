using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vdoflix.Web.Data;

namespace Vdoflix.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly Vdoflix.Web.Data.ApplicationDbContext _db;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, Vdoflix.Web.Data.ApplicationDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginVm { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = await _userManager.FindByEmailAsync(vm.Email) ?? await _userManager.FindByNameAsync(vm.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(vm);
        }
        var res = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: false);
        if (res.Succeeded)
            return Redirect(vm.ReturnUrl ?? Url.Action("Index", "Home")!);
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(vm);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null) => View(new RegisterVm { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
        var res = await _userManager.CreateAsync(user, vm.Password);
        if (res.Succeeded)
        {
            // seed a default profile
            _db.Profiles.Add(new Vdoflix.Web.Models.Profile { UserId = user.UserName!, Name = "Primary" });
            await _db.SaveChangesAsync();
            await _signInManager.SignInAsync(user, isPersistent: true);
            return Redirect(vm.ReturnUrl ?? Url.Action("Index", "Home")!);
        }
        foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public class LoginVm
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterVm
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match")] 
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }
}
