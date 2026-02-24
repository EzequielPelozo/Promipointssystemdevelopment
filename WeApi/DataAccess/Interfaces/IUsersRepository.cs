using Business.Entities;

namespace DataAccess.Interfaces;

public interface IUsersRepository : IRepository<User>
{
    Task<User?> GetByEmail(string email);
}
