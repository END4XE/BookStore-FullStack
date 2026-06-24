using System.ComponentModel.DataAnnotations;

namespace Final_Work.Models
{
    /// <summary>
    /// Модель, представляющая книгу в магазине.
    /// </summary>
    public class Books
    {
        /// <summary>
        /// Уникальный артикул книги (Stock Keeping Unit).
        /// </summary>
        [Key]
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Наименование книги.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Единица измерения товара.
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Стоимость книги.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Автор книги.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Производитель книги.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Категория, к которой относится книга.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Действующая скидка на товар.
        /// </summary>
        public int? Discount { get; set; }

        /// <summary>
        /// Количество доступных единиц товара на складе.
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Подробное текстовое описание книги.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Имя файла изображения книги для отображения в интерфейсе.
        /// </summary>
        public string? ImageName { get; set; }
    }
}