using Microsoft.EntityFrameworkCore;

namespace Final_Work.Models
{
    /// <summary>
    /// Контекст базы данных Entity Framework Core, инкапсулирующий подключение и сопоставление сущностей с таблицами СУБД.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AppDbContext"/> с конфигурационными параметрами.
        /// </summary>
        /// <param name="options">Параметры подключения и настройки провайдера базы данных.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Набор зарегистрированных пользователей системы.
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Набор книг, доступных для просмотра и заказа.
        /// </summary>
        public DbSet<Books> Books { get; set; }

        /// <summary>
        /// Набор оформленных заказов в магазине.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Набор связанных позиций товаров в рамках совершенных заказов.
        /// </summary>
        public DbSet<OrderProduct> OrderProducts { get; set; }

        /// <summary>
        /// Переопределение метода конфигурации моделей для создания точных связей, составных ключей и имен таблиц.
        /// </summary>
        /// <param name="modelBuilder">Построитель моделей, используемый для Fluent API конфигурации сущностей.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Books>().HasKey(b => b.SKU);

            modelBuilder.Entity<Users>().HasKey(u => u.UserID);

            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderID, op.ProductSKU });

            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderProduct>().ToTable("OrderItems");
        }
    }
}