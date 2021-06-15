using ProjectManagementSystem.Models;
using System.Linq;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public IQueryable<ApplicationUser> GetAllQueryable();
    }
}
