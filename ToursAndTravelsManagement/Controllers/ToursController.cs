using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class ToursController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToursController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Tours
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var userName = User?.Identity?.Name ?? "Unknown User"; // Get current logged-in user

            Log.Information("User {UserName} accessed the Tours Index page", userName);

            int pageIndex = pageNumber ?? 1;
            int size = pageSize ?? 10;

            var tours = await _unitOfWork.TourRepository.GetPaginatedAsync(pageIndex, size, "Destination");
            return View(tours);
        }

        // GET: Tours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Tour Details with null ID", userName);
                return NotFound();
            }

            var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value, "Destination");
            if (tour == null)
            {
                Log.Warning("User {UserName} tried to access Tour Details with invalid ID {TourId}", userName, id);
                return NotFound();
            }

            Log.Information("User {UserName} accessed details of Tour {TourId}", userName, id);
            return View(tour);
        }

        // GET: Tours/Create
        public async Task<IActionResult> Create()
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
            ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name");

            Log.Information("User {UserName} is accessing the Tour Creation page", userName);
            return View();
        }

        // POST: Tours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Price,MaxParticipants,DestinationId,CreatedBy,CreatedDate,IsActive")] Tour tour)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (ModelState.IsValid)
            {
                await _unitOfWork.TourRepository.AddAsync(tour);
                await _unitOfWork.CompleteAsync();

                Log.Information("User {UserName} created a new tour: {@Tour}", userName, tour);
                return RedirectToAction(nameof(Index));
            }

            var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
            ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);

            Log.Warning("User {UserName} attempted to create a tour with invalid data: {@Tour}", userName, tour);
            return View(tour);
        }

        // GET: Tours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Tour Edit with null ID", userName);
                return NotFound();
            }

            var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value);
            if (tour == null)
            {
                Log.Warning("User {UserName} tried to access Tour Edit with invalid ID {TourId}", userName, id);
                return NotFound();
            }

            var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
            ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);

            Log.Information("User {UserName} is editing Tour {TourId}", userName, id);
            return View(tour);
        }

        // POST: Tours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TourId,Name,Description,StartDate,EndDate,Price,MaxParticipants,DestinationId,CreatedBy,CreatedDate,IsActive")] Tour tour)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id != tour.TourId)
            {
                Log.Warning("User {UserName} tried to edit a tour with mismatched ID {TourId}", userName, id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.TourRepository.Update(tour);
                    await _unitOfWork.CompleteAsync();

                    Log.Information("User {UserName} successfully edited Tour {TourId}", userName, id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.TourId))
                    {
                        Log.Error("User {UserName} attempted to edit a non-existent tour {TourId}", userName, id);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
            ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);

            Log.Warning("User {UserName} submitted invalid data for editing Tour {TourId}", userName, id);
            return View(tour);
        }

        // GET: Tours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Tour Delete with null ID", userName);
                return NotFound();
            }

            var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value);
            if (tour == null)
            {
                Log.Warning("User {UserName} tried to access Tour Delete with invalid ID {TourId}", userName, id);
                return NotFound();
            }

            Log.Information("User {UserName} is deleting Tour {TourId}", userName, id);
            return View(tour);
        }

        // POST: Tours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            var tour = await _unitOfWork.TourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                Log.Warning("User {UserName} tried to delete a non-existent tour {TourId}", userName, id);
                return NotFound();
            }

            _unitOfWork.TourRepository.Remove(tour);
            await _unitOfWork.CompleteAsync();

            Log.Information("User {UserName} successfully deleted Tour {TourId}", userName, id);
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _unitOfWork.TourRepository.GetByIdAsync(id) != null;
        }
    }
}
