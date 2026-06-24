using Final_Work.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_Work.Controllers
{
    /// <summary>
    /// Контроллер для обработки запросов аутентификации и авторизации пользователей.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthController"/> с внедрением контекста базы данных.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Выполняет аутентификацию пользователя по переданным учетным данным.
        /// </summary>
        /// <param name="request">Модель запроса, содержащая логин и пароль пользователя.</param>
        /// <returns>Объект с идентификаторами пользователя и его роли в случае успешной проверки, иначе — ошибка доступа.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Логин и пароль обязательны для заполнения." });
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Login == request.Login && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль." });
            }

            return Ok(new
            {
                roleId = user.RoleID,
                userID = user.UserID,
                fullName = user.FullName
            });
        }
    }
}