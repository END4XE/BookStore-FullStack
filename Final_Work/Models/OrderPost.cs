namespace Final_Work.Models
{
    /// <summary>
    /// Объект переноса данных (DTO) для отправки и создания новой записи заказа на сервере.
    /// </summary>
    public class OrderPost
    {
        /// <summary>
        /// Номер создаваемого заказа (может быть null, если генерируется базой автоматически).
        /// </summary>
        public int? OrderID { get; set; }

        /// <summary>
        /// Дата создания заказа. По умолчанию инициализируется текущим системным временем.
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Расчетная дата доставки. По умолчанию выставляется на 3 дня вперед от текущей даты.
        /// </summary>
        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddDays(3);

        /// <summary>
        /// Идентификатор авторизованного клиента (null для заказов от гостей).
        /// </summary>
        public int? ClientID { get; set; }

        /// <summary>
        /// Уникальный секретный код для получения заказа в пункте выдачи.
        /// </summary>
        public int PickupCode { get; set; }

        /// <summary>
        /// Статус создаваемого заказа. По умолчанию принимает значение "Новый".
        /// </summary>
        public string Status { get; set; } = "Новый";

        /// <summary>
        /// Список позиций книг и их количества, входящих в состав отправляемого заказа.
        /// </summary>
        public List<OrderProduct> OrderProducts { get; set; } = new();
    }
}