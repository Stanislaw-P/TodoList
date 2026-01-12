using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoList.db.Models;
using TodoList.db.Repositories;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoitemsRepository _todoitemsRepository;

        public TodoController(ITodoitemsRepository todoitemsRepository)
        {
            _todoitemsRepository = todoitemsRepository;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var todoItems = await _todoitemsRepository.GetAllAsync(userId);

            return View(todoItems);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _todoitemsRepository.ToggleStatusAsync(id);
                var todoItems = await _todoitemsRepository.GetAllAsync(userId);
                return PartialView("_TaskList", todoItems);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка БД: " + ex.Message);
            }
        }

        public async Task<IActionResult> GetTasksPartial(string sortOrder)
        {

            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var todoItems = await _todoitemsRepository.GetAllAsync(userId);


            todoItems = sortOrder switch
            {
                "title" => todoItems.OrderBy(t => t.Title).ToList(),
                "date" => todoItems.OrderBy(t => t.CreatedAt).ToList(),
                _ => todoItems.OrderBy(t => t.IsCompleted).ToList()
            };

            return PartialView("_TaskList", todoItems);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateTodoItemViewModel model)
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var userId = Convert.ToInt32(userIdString);

            if (ModelState.IsValid)
            {
                try
                {
                    var newTask = new TodoItem
                    {
                        Title = model.Title.Trim(),
                        Description = model?.Description?.Trim(),
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    await _todoitemsRepository.AddAsync(newTask);
                    var todoItems = await _todoitemsRepository.GetAllAsync(userId);
                    return PartialView("_TaskList", todoItems);
                }
                catch (Exception ex)
                {
                    return BadRequest("Ошибка БД: " + ex.Message);
                }
            }

            return BadRequest("Некорректные данные. Название обязательно.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int todoItemId)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _todoitemsRepository.DeleteAsync(todoItemId);
                var todoItems = await _todoitemsRepository.GetAllAsync(userId);
                return PartialView("_TaskList", todoItems);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка БД: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditTodoItemViewModel model)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (ModelState.IsValid)
            {
                var existingTodoItem = await _todoitemsRepository.TryGetByIdAsync(model.Id);
                
                if (existingTodoItem == null)
                    return BadRequest($"Записи с id: {model.Id} - не существует.");

                var todoItem = new TodoItem
                {
                    Id = model.Id,
                    Title = model.Title.Trim(),
                    Description = model?.Description?.Trim()
                };

                await _todoitemsRepository.UpdateAsync(todoItem);
                var todoItems = await _todoitemsRepository.GetAllAsync(userId);
                return PartialView("_TaskList", todoItems);
            }

            return BadRequest("Некорректные данные. Название обязательно.");
        }
    }
}
