using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [MinLength(3, ErrorMessage ="Минимальная длина 3 символа")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Обязательное поле")]
        [MinLength(6, ErrorMessage = "Минимальная длина 6 символа")]
        public string Password { get; set; } = null!;

        
        [Required(ErrorMessage = "Обязательное поле")]
        public string ConfirmPassword { get; set; } = null!;
     
        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Введите корректный email")] public string Email { get; set; } = null!;
    }
}
