using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface IUsersRepository
    {
        Task<int> AddAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User> GetByEmailAsync(string email);
    }
}