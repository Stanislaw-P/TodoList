using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public interface ITodoitemsRepository
    {
        Task AddAsync(TodoItem newTodoItem);
        Task DeleteAsync(int TodoItemId);
        Task<List<TodoItem>> GetAllAsync(int userId);
        Task<List<DateTime>> GetDueDatesWithPendingTasksAsync(int userId);
        Task ToggleStatusAsync(int todoItemId);
        Task<TodoItem?> TryGetByIdAsync(int id);
        Task<List<TodoItem>?> TryGetForSelectedDateAsync(int userId, DateTime? selectedDate);
        Task UpdateAsync(TodoItem editedTodoItem);
    }
}