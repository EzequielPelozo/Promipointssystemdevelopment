using Business.Entities;

namespace DataAccess.Interfaces;

public interface IAssignmentsRepository : IRepository<PointAssignment>
{
    Task<IEnumerable<PointAssignment>> GetByReceiver(int userId);
    Task<IEnumerable<PointAssignment>> GetAllWithUsers();
}
