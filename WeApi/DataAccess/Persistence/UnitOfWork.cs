using DataAccess.Context;
using DataAccess.Interfaces;

namespace DataAccess.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PromiPointsDbContext _context;
    private bool _disposed;

    public IUsersRepository Users { get; }
    public IAllocationsRepository Allocations { get; }
    public IAssignmentsRepository Assignments { get; }

    public UnitOfWork(PromiPointsDbContext context)
    {
        _context = context;
        Users = new UsersRepository(_context);
        Allocations = new AllocationsRepository(_context);
        Assignments = new AssignmentsRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                _context?.Dispose();
            _disposed = true;
        }
    }
}
