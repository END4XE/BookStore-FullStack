using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreClient.Models
{
    /// <summary>
    /// Модель, описывающая отдельную позицию товара (книгу) и её количество в составе заказа.
    /// </summary>
    public class OrderProduct
    {
        [Column("SKU")]
        /// <summary>
        /// Уникальный артикул книги (Stock Keeping Unit).
        /// </summary>
        public string ProductSKU { get; set; } = string.Empty;

        /// <summary>
        /// Количество единиц данного товара в позиции заказа.
        /// </summary>
        public int Quantity { get; set; }
    }
}
