using Business.Entities;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Persistence;

public class UsersRepository(PromiPointsDbContext context) : Repository<User>(context), IUsersRepository
{
    private readonly PromiPointsDbContext _context = context;

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
