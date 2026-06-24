namespace BookStoreClient.Models
{
    /// <summary>
    /// Модель передачи данных (DTO) для отправки нового заказа на сервер API.
    /// </summary>
    public class OrderPost
    {
        /// <summary>
        /// Уникальный идентификатор (номер) заказа.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Дата и время оформления заказа. По умолчанию инициализируется текущим временем.
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Планируемая дата доставки. По умолчанию рассчитывается на 3 дня вперед от даты заказа.
        /// </summary>
        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddDays(3);

        /// <summary>
        /// Идентификатор авторизованного клиента. Может быть null, если заказ оформляет гость.
        /// </summary>
        public int? ClientID { get; set; }

        /// <summary>
        /// Код получения заказа в пункте выдачи.
        /// </summary>
        public int PickupCode { get; set; }

        /// <summary>
        /// Текущий статус обработки заказа. По умолчанию принимает значение "Новый".
        /// </summary>
        public string Status { get; set; } = "Новый";

        /// <summary>
        /// Список позиций товаров (книг), входящих в состав данного заказа.
        /// </summary>
        public List<OrderProduct> OrderProducts { get; set; } = new();
    }
}