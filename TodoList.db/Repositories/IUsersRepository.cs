using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface IUsersRepository
    {
        Task<List<User>> GetAllAsync();
    }
}