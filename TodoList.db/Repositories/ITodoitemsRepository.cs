using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface ITodoitemsRepository
    {
        Task<List<TodoItem>> GetAllAsync();
    }
}