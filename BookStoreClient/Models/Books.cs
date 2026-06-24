using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreClient.Models
{
    /// <summary>
    /// Модель книги для отображения и использования в интерфейсе WPF-клиента.
    /// </summary>
    [Table("Books")]
    public class Books
    {
        /// <summary>
        /// Уникальный артикул книги (Stock Keeping Unit).
        /// </summary>
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
        public decimal? Price { get; set; }

        /// <summary>
        /// Автор книги.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Издательство, выпустившее книгу.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Категория (жанр) книги.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Размер скидки на книгу.
        /// </summary>
        public int? Discount { get; set; }

        /// <summary>
        /// Доступное количество экземпляров на складе.
        /// </summary>
        public int? StockQuantity { get; set; }

        /// <summary>
        /// Текстовое описание книги.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Имя файла обложки книги.
        /// </summary>
        public string? ImageName { get; set; }

        /// <summary>
        /// Вычисляемый путь к изображению обложки для привязки (Binding) в XAML.
        /// Если имя файла не указано, возвращает путь к заглушке по умолчанию.
        /// </summary>
        public string ImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageName))
                {
                    return "/Images/picture.png";
                }

                return $"/Images/{ImageName}";
            }
        }
    }
}