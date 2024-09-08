using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers;

public class DestinationsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DestinationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Destinations
    public async Task<IActionResult> Index()
    {
        var destinations = await _unitOfWork.DestinationRepository.GetAllAsync();
        return View(destinations);
    }

    // GET: Destinations/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
        if (destination == null)
        {
            return NotFound();
        }

        return View(destination);
    }

    // GET: Destinations/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Destinations/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Country,City,ImageUrl,CreatedBy,CreatedDate,IsActive")] Destination destination)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.DestinationRepository.AddAsync(destination);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(destination);
    }

    // GET: Destinations/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
        if (destination == null)
        {
            return NotFound();
        }

        return View(destination);
    }

    // POST: Destinations/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DestinationId,Name,Description,Country,City,ImageUrl,CreatedBy,CreatedDate,IsActive")] Destination destination)
    {
        if (id != destination.DestinationId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.DestinationRepository.Update(destination);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(destination.DestinationId))
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
        return View(destination);
    }

    // GET: Destinations/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
        if (destination == null)
        {
            return NotFound();
        }

        return View(destination);
    }

    // POST: Destinations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id);
        if (destination == null)
        {
            return NotFound();
        }

        _unitOfWork.DestinationRepository.Remove(destination);
        await _unitOfWork.CompleteAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DestinationExists(int id)
    {
        return _unitOfWork.DestinationRepository.GetByIdAsync(id) != null;
    }
}