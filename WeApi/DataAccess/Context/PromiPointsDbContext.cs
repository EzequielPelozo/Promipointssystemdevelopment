using Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context;

public class PromiPointsDbContext(DbContextOptions<PromiPointsDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<MonthlyAllocation> MonthlyAllocations { get; set; }
    public DbSet<PointAssignment> PointAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<MonthlyAllocation>()
            .HasIndex(a => new { a.UserId, a.Month })
            .IsUnique();

        modelBuilder.Entity<PointAssignment>()
            .HasOne(pa => pa.FromUser)
            .WithMany(u => u.SentAssignments)
            .HasForeignKey(pa => pa.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PointAssignment>()
            .HasOne(pa => pa.ToUser)
            .WithMany(u => u.ReceivedAssignments)
            .HasForeignKey(pa => pa.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed demo data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Password hash for "Promi2024!"
        const string passwordHash = "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa";

        var users = new[]
        {
            new User { Id = 1, Name = "María García", Email = "maria.garcia@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Desarrollo", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 2, Name = "Juan Pérez", Email = "juan.perez@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Marketing", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 3, Name = "Ana Rodríguez", Email = "ana.rodriguez@grupoprominente.com", PasswordHash = passwordHash, Role = "people", Department = "People & Culture", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 4, Name = "Carlos López", Email = "carlos.lopez@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Ventas", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 5, Name = "Laura Martínez", Email = "laura.martinez@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Desarrollo", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 6, Name = "Diego Sánchez", Email = "diego.sanchez@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Operaciones", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 7, Name = "Sofia Torres", Email = "sofia.torres@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Marketing", CreatedAt = new DateTime(2026, 1, 1) },
            new User { Id = 8, Name = "Roberto Flores", Email = "roberto.flores@grupoprominente.com", PasswordHash = passwordHash, Role = "employee", Department = "Ventas", CreatedAt = new DateTime(2026, 1, 1) },
        };

        modelBuilder.Entity<User>().HasData(users);

        const string month = "2026-02";

        var allocations = new[]
        {
            new MonthlyAllocation { Id = 1, UserId = 1, Month = month, PointsRemaining = 5, PointsReceived = 5 },
            new MonthlyAllocation { Id = 2, UserId = 2, Month = month, PointsRemaining = 6, PointsReceived = 4 },
            new MonthlyAllocation { Id = 3, UserId = 3, Month = month, PointsRemaining = 10, PointsReceived = 0 },
            new MonthlyAllocation { Id = 4, UserId = 4, Month = month, PointsRemaining = 8, PointsReceived = 2 },
            new MonthlyAllocation { Id = 5, UserId = 5, Month = month, PointsRemaining = 6, PointsReceived = 0 },
            new MonthlyAllocation { Id = 6, UserId = 6, Month = month, PointsRemaining = 10, PointsReceived = 0 },
            new MonthlyAllocation { Id = 7, UserId = 7, Month = month, PointsRemaining = 10, PointsReceived = 0 },
            new MonthlyAllocation { Id = 8, UserId = 8, Month = month, PointsRemaining = 10, PointsReceived = 0 },
        };

        modelBuilder.Entity<MonthlyAllocation>().HasData(allocations);

        var assignments = new[]
        {
            new PointAssignment { Id = 1, FromUserId = 2, ToUserId = 1, Points = 3, Category = "Trabajo en equipo", Message = "¡Excelente colaboración en el proyecto!", Timestamp = 1740268800000L, Month = month },
            new PointAssignment { Id = 2, FromUserId = 4, ToUserId = 1, Points = 2, Category = "Innovación", Message = "Gran idea para mejorar el proceso", Timestamp = 1739836800000L, Month = month },
            new PointAssignment { Id = 3, FromUserId = 5, ToUserId = 2, Points = 4, Category = "Liderazgo", Message = null, Timestamp = 1740096000000L, Month = month },
            new PointAssignment { Id = 4, FromUserId = 1, ToUserId = 4, Points = 2, Category = "Colaboración", Message = "Siempre dispuesto a ayudar", Timestamp = 1740355200000L, Month = month },
        };

        modelBuilder.Entity<PointAssignment>().HasData(assignments);
    }
}
