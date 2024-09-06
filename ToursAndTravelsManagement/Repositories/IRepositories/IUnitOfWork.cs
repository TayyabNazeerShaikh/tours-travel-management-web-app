using ToursAndTravelsManagement.Models;

namespace ToursAndTravelsManagement.Repositories.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Tour> Tours { get; }
    IRepository<Destination> Destinations { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<User> Users { get; }
    Task<int> SaveAsync();
}
