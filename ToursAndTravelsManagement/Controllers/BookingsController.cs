using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers;
public class BookingsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    // GET: Bookings
    public async Task<IActionResult> Index()
    {
        var bookings = await _unitOfWork.BookingRepository.GetAllAsync("User,Tour");
        return View(bookings);
    }

    // GET: Bookings/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id.Value, "User,Tour");
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // GET: Bookings/Create
    public async Task<IActionResult> Create()
    {
        var users = await _userManager.Users.ToListAsync();
        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        ViewBag.TourId = new SelectList(await _unitOfWork.TourRepository.GetAllAsync(), "TourId", "Name");
        return View();
    }

    // POST: Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserId,TourId,BookingDate,NumberOfParticipants,TotalPrice,Status,PaymentMethod,PaymentStatus,CreatedBy,CreatedDate,IsActive")] Booking booking)
    {
        if (ModelState.IsValid)
        {
            // Automatically populate CreatedBy with the current user's ID
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                booking.CreatedBy = currentUser.Id;
            }

            booking.CreatedDate = DateTime.Now;
            booking.IsActive = true;
            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        var users = await _userManager.Users.ToListAsync();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", booking.UserId);
        ViewBag.TourId = new SelectList(await _unitOfWork.TourRepository.GetAllAsync(), "TourId", "Name", booking.TourId);
        return View(booking);
    }

    // GET: Bookings/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id.Value, "User,Tour");
        if (booking == null)
        {
            return NotFound();
        }

        var users = await _userManager.Users.ToListAsync();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", booking.UserId);
        ViewBag.TourId = new SelectList(await _unitOfWork.TourRepository.GetAllAsync(), "TourId", "Name", booking.TourId);
        return View(booking);
    }

    // POST: Bookings/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookingId,UserId,TourId,BookingDate,NumberOfParticipants,TotalPrice,Status,PaymentMethod,PaymentStatus,CreatedBy,CreatedDate,IsActive")] Booking booking)
    {
        if (id != booking.BookingId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        var users = await _userManager.Users.ToListAsync();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", booking.UserId);
        ViewBag.TourId = new SelectList(await _unitOfWork.TourRepository.GetAllAsync(), "TourId", "Name", booking.TourId);
        return View(booking);
    }

    // GET: Bookings/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id.Value, "User,Tour");
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // POST: Bookings/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        _unitOfWork.BookingRepository.Remove(booking);
        await _unitOfWork.CompleteAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int id)
    {
        return _unitOfWork.BookingRepository.GetByIdAsync(id) != null;
    }
}