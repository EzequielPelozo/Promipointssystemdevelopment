namespace DataAccess.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUsersRepository Users { get; }
    IAllocationsRepository Allocations { get; }
    IAssignmentsRepository Assignments { get; }
    Task<int> SaveChangesAsync();
}
