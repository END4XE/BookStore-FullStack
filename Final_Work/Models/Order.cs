using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Work.Models
{
    /// <summary>
    /// Модель, представляющая оформленный заказ в магазине.
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        /// <summary>
        /// Уникальный номер заказа.
        /// </summary>
        [Key]
        public int? OrderID { get; set; }

        /// <summary>
        /// Дата и время создания (сохранения) заказа.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Расчетная дата доставки заказа в пункт выдачи.
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Идентификатор клиента, сделавшего заказ. Имеет значение null для гостей.
        /// </summary>
        public int? ClientID { get; set; }

        /// <summary>
        /// Трехзначный код получения заказа.
        /// </summary>
        public int PickupCode { get; set; }

        /// <summary>
        /// Текущий статус обработки заказа.
        /// </summary>
        public string Status { get; set; } = "Новый";
    }
}