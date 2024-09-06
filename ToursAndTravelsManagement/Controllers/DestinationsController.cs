using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index()
    {
        var destinations = await _unitOfWork.Destinations.GetAllAsync();
        return View(destinations);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Destination destination)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Destinations.AddAsync(destination);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(destination);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var destination = await _unitOfWork.Destinations.GetByIdAsync(id.Value);

        if (destination == null)
        {
            return NotFound();
        }

        return View(destination);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Destination destination)
    {
        if (id != destination.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Destinations.Update(destination);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(destination);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var destination = await _unitOfWork.Destinations.GetByIdAsync(id.Value);

        if (destination == null)
        {
            return NotFound();
        }

        return View(destination);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var destination = await _unitOfWork.Destinations.GetByIdAsync(id);
        _unitOfWork.Destinations.Delete(destination);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}
