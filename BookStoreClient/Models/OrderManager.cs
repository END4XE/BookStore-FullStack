namespace BookStoreClient.Models
{
    /// <summary>
    /// Статический менеджер для централизованного управления состоянием текущего заказа
    /// и хранения данных авторизованной сессии клиента в WPF-приложении.
    /// </summary>
    public static class OrderManager
    {
        /// <summary>
        /// Список всех товарных позиций, добавленных в текущий заказ.
        /// </summary>
        public static List<OrderItem> CurrentOrder { get; set; } = new();

        /// <summary>
        /// Идентификатор текущего авторизованного клиента. Принимает значение null для гостей.
        /// </summary>
        public static int? CurrentClientID { get; set; } = null;

        /// <summary>
        /// Полное имя (ФИО) текущего клиента. По умолчанию инициализируется как "Гость".
        /// </summary>
        public static string CurrentClientFIO { get; set; } = "Гость";

        /// <summary>
        /// Добавляет одну единицу товара (книги) в текущий заказ.
        /// Если товар уже присутствует в корзине, увеличивает его количество на 1.
        /// </summary>
        /// <param name="book">Объект добавляемой книги.</param>
        public static void AddProduct(Books book)
        {
            if (book == null || string.IsNullOrEmpty(book.SKU))
            {
                return;
            }

            var existingItem = CurrentOrder.FirstOrDefault(item => item.Product?.SKU == book.SKU);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                CurrentOrder.Add(new OrderItem { Product = book, Quantity = 1 });
            }
        }

        /// <summary>
        /// Полностью удаляет товарную позицию из текущего заказа по уникальному артикулу книги (SKU).
        /// </summary>
        /// <param name="bookSKU">Артикул книги, которую необходимо убрать из корзины.</param>
        public static void RemoveProduct(string bookSKU)
        {
            if (string.IsNullOrWhiteSpace(bookSKU))
            {
                return;
            }

            var existingItem = CurrentOrder.FirstOrDefault(item => item.Product?.SKU == bookSKU);
            if (existingItem != null)
            {
                CurrentOrder.Remove(existingItem);
            }
        }

        /// <summary>
        /// Вычисляет и возвращает итоговую стоимость всего текущего заказа.
        /// </summary>
        /// <returns>Суммарная стоимость всех позиций в корзине или null, если корзина пуста.</returns>
        public static decimal? GetOrderTotal()
        {
            return CurrentOrder.Sum(item => item.TotalPrice);
        }

        /// <summary>
        /// Вычисляет общее суммарное количество всех штук товаров, находящихся в корзине.
        /// </summary>
        /// <returns>Общее количество единиц товаров в заказе.</returns>
        public static int GetTotalItemsCount()
        {
            return CurrentOrder.Sum(item => item.Quantity);
        }
    }
}