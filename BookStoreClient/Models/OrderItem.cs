namespace BookStoreClient.Models
{
    /// <summary>
    /// Представляет отдельную товарную позицию в корзине покупок (выбранный товар и его количество).
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Ссылка на объект книги. Может принимать значение null.
        /// </summary>
        public Books? Product { get; set; }

        /// <summary>
        /// Количество единиц данного товара в текущей позиции корзины.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Вычисляемая общая стоимость данной позиции с учетом количества товара.
        /// Возвращает null, если объект товара не инициализирован.
        /// </summary>
        public decimal? TotalPrice => Product?.Price * Quantity;
    }
}
