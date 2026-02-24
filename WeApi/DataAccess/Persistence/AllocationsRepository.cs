using Business.Entities;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Persistence;

public class AllocationsRepository(PromiPointsDbContext context) : Repository<MonthlyAllocation>(context), IAllocationsRepository
{
    private readonly PromiPointsDbContext _context = context;

    public async Task<MonthlyAllocation?> GetByUserAndMonth(int userId, string month)
    {
        return await _context.MonthlyAllocations
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Month == month);
    }
}
