using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class CreateTodoItemViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
    }
}
