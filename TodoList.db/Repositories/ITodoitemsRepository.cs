using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface ITodoitemsRepository
    {
        Task AddAsync(TodoItem newTodoItem);
        Task DeleteAsync(int TodoItemId);
        Task<List<TodoItem>> GetAllAsync(int userId);
        Task ToggleStatusAsync(int todoItemId);
        Task<TodoItem?> TryGetByIdAsync(int id);
        Task UpdateAsync(TodoItem editedTodoItem);
    }
}