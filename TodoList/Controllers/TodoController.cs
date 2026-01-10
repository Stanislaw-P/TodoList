using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoList.db.Models;
using TodoList.db.Repositories;

namespace TodoList.Controllers
{
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
        public async Task<IActionResult> ToggleStatus()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todoItems = await _todoitemsRepository.GetAllAsync(userId);

            return View();
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

    }
}
