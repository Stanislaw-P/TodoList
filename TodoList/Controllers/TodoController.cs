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

        public async Task<IActionResult> Index()
        {
            //var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            //var todoItems = await _todoitemsRepository.GetAllAsync(userId);
            var todoItems = new List<TodoItem>()
            {
                new TodoItem { Title = "Test1", Description = "Desc1", CreatedAt = DateTime.Now, Id =1} ,
                new TodoItem { Title = "Test2", Description = "Desc2", CreatedAt = DateTime.Now, IsCompleted=true, Id =2} ,
                new TodoItem { Title = "Test3", Description = "Desc3", CreatedAt = DateTime.Now, Id =3}
            };
            return View(todoItems);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todoItems = await _todoitemsRepository.GetAllAsync(userId);

            return View();
        }

    }
}
