using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index()
    {
        var tours = await _unitOfWork.Tours.GetAllAsync();
        return View(tours);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Tour tour)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Tours.AddAsync(tour);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(tour);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tour = await _unitOfWork.Tours.GetByIdAsync(id.Value);

        if (tour == null)
        {
            return NotFound();
        }

        return View(tour);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Tour tour)
    {
        if (id != tour.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Tours.Update(tour);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(tour);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tour = await _unitOfWork.Tours.GetByIdAsync(id.Value);

        if (tour == null)
        {
            return NotFound();
        }

        return View(tour);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tour = await _unitOfWork.Tours.GetByIdAsync(id);
        _unitOfWork.Tours.Delete(tour);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}
