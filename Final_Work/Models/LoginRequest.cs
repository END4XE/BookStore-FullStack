namespace Final_Work.Models
{
    /// <summary>
    /// Объект запроса, содержащий учетные данные пользователя для прохождения авторизации.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Логин (псевдоним) пользователя в системе.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя для сверки на сервере.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}