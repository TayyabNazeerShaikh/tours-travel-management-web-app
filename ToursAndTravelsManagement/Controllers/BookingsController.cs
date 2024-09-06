using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers;

public class BookingsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _unitOfWork.Bookings.GetAllAsync();
        return View(bookings);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Tours"] = new SelectList(await _unitOfWork.Tours.GetAllAsync(), "Id", "Name");
        ViewData["Users"] = new SelectList(await _unitOfWork.Users.GetAllAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Booking booking)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["Tours"] = await _unitOfWork.Tours.GetAllAsync();
        ViewData["Users"] = await _unitOfWork.Users.GetAllAsync();
        return View(booking);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _unitOfWork.Bookings.GetByIdAsync(id.Value);

        if (booking == null)
        {
            return NotFound();
        }

        ViewData["Tours"] = new SelectList(await _unitOfWork.Tours.GetAllAsync(), "Id", "Name");

        var users = await _unitOfWork.Users.GetAllAsync();
        ViewData["Users"] = new SelectList(users.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Booking booking)
    {
        if (id != booking.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Tours"] = await _unitOfWork.Tours.GetAllAsync();
        ViewData["Users"] = await _unitOfWork.Users.GetAllAsync();
        return View(booking);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _unitOfWork.Bookings.GetByIdAsync(id.Value);

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        _unitOfWork.Bookings.Delete(booking);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}
