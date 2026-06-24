using System.ComponentModel.DataAnnotations;

namespace Final_Work.Models
{
    /// <summary>
    /// Модель, представляющая пользователя системы.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        [Key]
        public int UserID { get; set; }

        /// <summary>
        /// Уникальный логин пользователя для авторизации.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя в системе.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор роли.
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// Полное имя (ФИО) пользователя.
        /// </summary>
        public string? FullName { get; set; }
    }
}