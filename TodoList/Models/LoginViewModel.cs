using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage ="Введите корректный email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Обязательное поле")]
        public string Password { get; set; } = null!;
        
        public bool RememberMe { get; set; }
    }
}
