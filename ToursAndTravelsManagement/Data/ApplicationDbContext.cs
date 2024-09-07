using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToursAndTravelsManagement.Models;

namespace ToursAndTravelsManagement.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Tour> Tours { get; set; }
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}