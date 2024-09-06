using ToursAndTravelsManagement.Data;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRepository<Tour> Tours { get; private set; }
    public IRepository<Destination> Destinations { get; private set; }
    public IRepository<Booking> Bookings { get; private set; }
    public IRepository<User> Users { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Tours = new Repository<Tour>(_context);
        Destinations = new Repository<Destination>(_context);
        Bookings = new Repository<Booking>(_context);
        Users = new Repository<User>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
