using Business.Entities;

namespace DataAccess.Interfaces;

public interface IAllocationsRepository : IRepository<MonthlyAllocation>
{
    Task<MonthlyAllocation?> GetByUserAndMonth(int userId, string month);
}
