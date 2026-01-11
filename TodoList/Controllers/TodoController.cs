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


            //var todoItems = new List<TodoItem>()
            //{
            //    new TodoItem { Title = "BTest1", Description = "Desc1", CreatedAt = DateTime.Now, Id =1} ,
            //    new TodoItem { Title = "ETest2", Description = "Desc2", CreatedAt = DateTime.Now, IsCompleted=true, Id =2} ,
            //    new TodoItem { Title = "ATest3", Description = "Desc3", CreatedAt = DateTime.Now, Id =3},
            //    new TodoItem { Title = "DTest3", Description = "Desc4", CreatedAt = DateTime.Now, Id =4, IsCompleted=true}
            //};

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


            //var todoItems = new List<TodoItem>()
            //{
            //    new TodoItem { Title = "BTest1", Description = "Desc1", CreatedAt = DateTime.Now, Id =1} ,
            //    new TodoItem { Title = "ETest2", Description = "Desc2", CreatedAt = DateTime.Now, IsCompleted=true, Id =2} ,
            //    new TodoItem { Title = "ATest3", Description = "Desc3", CreatedAt = DateTime.Now, Id =3},
            //    new TodoItem { Title = "DTest3", Description = "Desc4", CreatedAt = DateTime.Now, Id =4, IsCompleted = true}
            //};

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
                        Title = model.Title,
                        Description = model.Description,
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
    }
}
