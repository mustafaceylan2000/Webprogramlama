using BarberApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Salon> Salons { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    //public DbSet<User> Users { get; set; }
    public DbSet<EmployeeService> EmployeeServices { get; set; }
    public DbSet<AppointmentService> AppointmentServices { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Çalışan ve Hizmetler arasında Many-to-Many ilişki
        modelBuilder.Entity<EmployeeService>()
            .HasKey(es => new { es.EmployeeId, es.ServiceId });

        modelBuilder.Entity<EmployeeService>()
            .HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeServices)
            .HasForeignKey(es => es.EmployeeId);

        modelBuilder.Entity<EmployeeService>()
            .HasOne(es => es.Service)
            .WithMany(s => s.EmployeeServices)
            .HasForeignKey(es => es.ServiceId);

        // Randevu ve Hizmetler arasında Many-to-Many ilişki
        modelBuilder.Entity<AppointmentService>()
            .HasKey(ee => new {ee.AppointmentId , ee.ServiceId });

        modelBuilder.Entity<AppointmentService>()
            .HasOne(ee => ee.Appointment)
            .WithMany(a => a.AppointmentServices)
            .HasForeignKey(ee => ee.AppointmentId);

        modelBuilder.Entity<AppointmentService>()
            .HasOne(ee => ee.Service)
            .WithMany(s => s.AppointmentServices)
            .HasForeignKey(ee => ee.ServiceId);
    }
}
