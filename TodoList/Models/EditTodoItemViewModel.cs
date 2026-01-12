using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class EditTodoItemViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
    }
}
