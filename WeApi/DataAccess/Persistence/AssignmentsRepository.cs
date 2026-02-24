using Business.Entities;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Persistence;

public class AssignmentsRepository(PromiPointsDbContext context) : Repository<PointAssignment>(context), IAssignmentsRepository
{
    private readonly PromiPointsDbContext _context = context;

    public async Task<IEnumerable<PointAssignment>> GetByReceiver(int userId)
    {
        return await _context.PointAssignments
            .AsNoTracking()
            .Where(a => a.ToUserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<PointAssignment>> GetAllWithUsers()
    {
        return await _context.PointAssignments
            .AsNoTracking()
            .Include(a => a.FromUser)
            .Include(a => a.ToUser)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
