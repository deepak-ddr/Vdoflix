using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vdoflix.Web.Data;
using Vdoflix.Web.Models;

namespace Vdoflix.Web.Controllers;

[Authorize]
public class ProfilesController : Controller
{
    private readonly ApplicationDbContext _db;
    private const string SessionKey = "ProfileId";

    public ProfilesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.Identity!.Name!;
        var profiles = await _db.Profiles.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.CurrentProfileId = HttpContext.Session.GetInt32(SessionKey);
        return View(profiles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name)
    {
        var userId = User.Identity!.Name!;
        if (string.IsNullOrWhiteSpace(name)) name = "Profile";
        _db.Profiles.Add(new Profile { Name = name, UserId = userId });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Select(int id)
    {
        HttpContext.Session.SetInt32(SessionKey, id);
        return RedirectToAction("Index", "Home");
    }
}
