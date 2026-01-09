using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TodoList.db.Models;
using TodoList.db.Repositories;
using TodoList.Models;

namespace TodoList.Controllers
{
    public class AccountController : Controller
    {
        readonly IUsersRepository _usersRepository;

        public AccountController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            User existingUser = await _usersRepository.GetByEmailAsync(model.Email);
            string passHash = GetSha256Hash(model.Password);

            if (existingUser != null && existingUser.PasswordHash == passHash)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existingUser.Name),
                    new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Настройка параметров аутентификации
                var authProperties = new AuthenticationProperties
                {
                    // Если true, кука сохранится после закрытия браузера
                    IsPersistent = model.RememberMe,

                    // Устанавливаем срок жизни (например, на 14 дней)
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
                };

                // Записываем Cookie в браузер
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
                ModelState.AddModelError("", "Пароли должны совпадать");

            if (ModelState.IsValid)
            {
                string passHash = GetSha256Hash(model.Password);

                User newUser = new User
                {
                    Name = model.Name,
                    PasswordHash = passHash,
                    Email = model.Email
                };

                int newUserId = await _usersRepository.AddAsync(newUser);

                if (newUserId > 0)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, newUser.Name),
                        new Claim(ClaimTypes.NameIdentifier, newUserId.ToString())
                     };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Настраиваем куки (сразу применяем Remember Me)
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Ошибка при создании аккаунта");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private string GetSha256Hash(string pass)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);

            byte[] hashBytes = SHA256.HashData(passBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
