using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ToursAndTravelsManagement.Data;
using ToursAndTravelsManagement.Models;

namespace ToursAndTravelsManagement.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageUsers()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    public IActionResult EditUser(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(ManageUsers));
        }
        return View(user);
    }

    public IActionResult DeleteUser(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction(nameof(ManageUsers));
    }
}