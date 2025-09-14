using ToDo.Core.Entities;

namespace ToDo.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);

    }
}
