namespace BookStoreClient.Models
{
    /// <summary>
    /// Объект переноса данных (DTO) для обновления существующего заказа сотрудником.
    /// </summary>
    public class OrderUpdateDto
    {
        /// <summary>
        /// Новая расчетная дата доставки заказа.
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Новый статус обработки заказа (например: Новый, К получению, Завершен).
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}