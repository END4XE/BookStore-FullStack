using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Work.Models
{
    /// <summary>
    /// Модель, представляющая связь между заказом и товарами.
    /// </summary>
    [Table("OrderItems")]
    public class OrderProduct
    {
        /// <summary>
        /// Номер заказа. Входит в составной первичный ключ.
        /// </summary>
        public int OrderID { get; set; }

        [Column("SKU")]
        /// <summary>
        /// Артикул выбранного товара. Входит в составной первичный ключ.
        /// </summary>
        public string ProductSKU { get; set; } = string.Empty;

        /// <summary>
        /// Количество единиц данного товара в составе текущего заказа.
        /// </summary>
        public int Quantity { get; set; }
    }
}