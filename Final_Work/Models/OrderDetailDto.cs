namespace Final_Work.Models
{
    /// <summary>
    /// Объект переноса данных (DTO) с подробной информацией о заказе для передачи на сторону клиента.
    /// </summary>
    public class OrderDetailDto
    {
        /// <summary>
        /// Уникальный идентификатор заказа.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Дата и время создания заказа.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Расчетная дата доставки заказа в пункт выдачи.
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Идентификатор клиента, сделавшего заказ (принимает значение null для гостей).
        /// </summary>
        public int? ClientID { get; set; }

        /// <summary>
        /// Полное имя (ФИО) клиента или статус неавторизованного пользователя.
        /// </summary>
        public string ClientFullName { get; set; } = string.Empty;

        /// <summary>
        /// Трехзначный числовой код получения заказа.
        /// </summary>
        public int PickupCode { get; set; }

        /// <summary>
        /// Текущий статус обработки заказа.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}