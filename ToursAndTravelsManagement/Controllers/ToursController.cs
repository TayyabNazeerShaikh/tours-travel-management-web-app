using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers;
public class ToursController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ToursController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Tours
    public async Task<IActionResult> Index()
    {
        var tours = await _unitOfWork.TourRepository.GetAllAsync("Destination");
        return View(tours);
    }

    // GET: Tours/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value, "Destination");
        if (tour == null)
        {
            return NotFound();
        }

        return View(tour);
    }

    // GET: Tours/Create
    public async Task<IActionResult> Create()
    {
        var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
        ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name");
        return View();
    }

    // POST: Tours/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Price,MaxParticipants,DestinationId,CreatedBy,CreatedDate,IsActive")] Tour tour)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.TourRepository.AddAsync(tour);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
        ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);
        return View(tour);
    }

    // GET: Tours/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value);
        if (tour == null)
        {
            return NotFound();
        }

        var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
        ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);
        return View(tour);
    }

    // POST: Tours/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TourId,Name,Description,StartDate,EndDate,Price,MaxParticipants,DestinationId,CreatedBy,CreatedDate,IsActive")] Tour tour)
    {
        if (id != tour.TourId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.TourRepository.Update(tour);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourExists(tour.TourId))
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
        var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
        ViewBag.DestinationId = new SelectList(destinations, "DestinationId", "Name", tour.DestinationId);
        return View(tour);
    }

    // GET: Tours/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tour = await _unitOfWork.TourRepository.GetByIdAsync(id.Value);
        if (tour == null)
        {
            return NotFound();
        }

        return View(tour);
    }

    // POST: Tours/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tour = await _unitOfWork.TourRepository.GetByIdAsync(id);
        if (tour == null)
        {
            return NotFound();
        }

        _unitOfWork.TourRepository.Remove(tour);
        await _unitOfWork.CompleteAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TourExists(int id)
    {
        return _unitOfWork.TourRepository.GetByIdAsync(id) != null;
    }
}