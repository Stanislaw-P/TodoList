using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface ITodoitemsRepository
    {
        Task AddAsync(TodoItem newTodoItem);
        Task<List<TodoItem>> GetAllAsync(int userId);
        Task ToggleStatusAsync(int todoItemId);
    }
}