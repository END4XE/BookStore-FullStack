using Final_Work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Work.Controllers
{
    /// <summary>
    /// Контроллер для управления заказами магазина.
    /// Обеспечивает получение информации о заказах, добавление новых заказов и обновление их статусов.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="OrdersController"/> с внедрением контекста базы данных.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает максимальный существующий идентификатор заказа в системе.
        /// </summary>
        /// <returns>Максимальный ID заказа или 0, если список заказов пуст.</returns>
        [HttpGet("max-id")]
        public async Task<ActionResult<int>> GetMaxOrderId()
        {
            var maxId = await _context.Orders.MaxAsync(o => (int?)o.OrderID) ?? 0;
            return Ok(maxId);
        }

        /// <summary>
        /// Получает подробную информацию о заказе по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор заказа.</param>
        /// <returns>Объект с подробной информацией о заказе DTO или сообщение об ошибке.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Некорректный идентификатор заказа." });
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound(new { message = "Заказ с указанным номером не найден." });
            }

            string clientFullName = string.Empty;

            if (order.ClientID.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == order.ClientID.Value);
                clientFullName = user?.FullName ?? string.Empty;
            }

            var detailDto = new OrderDetailDto
            {
                OrderID = order.OrderID ?? 0,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                ClientID = order.ClientID,
                ClientFullName = clientFullName,
                PickupCode = order.PickupCode,
                Status = order.Status
            };

            return Ok(detailDto);
        }

        /// <summary>
        /// Обновляет параметры существующего заказа (дату доставки и статус).
        /// </summary>
        /// <param name="id">Идентификатор изменяемого заказа.</param>
        /// <param name="dto">Объект с обновленными данными заказа.</param>
        /// <returns>Статус выполнения операции и информационное сообщение.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto dto)
        {
            if (id <= 0 || dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { message = "Некорректные или неполные данные для обновления заказа." });
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound(new { message = "Заказ не найден." });
            }

            order.DeliveryDate = dto.DeliveryDate;
            order.Status = dto.Status;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Данные заказа успешно обновлены." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при обновлении заказа: {ex.Message}" });
            }
        }

        /// <summary>
        /// Создает новый заказ в базе данных совместно со связанными позициями товаров в рамках единой транзакции.
        /// </summary>
        /// <param name="dto">Объект с комплексными данными для создания нового заказа.</param>
        /// <returns>Результат транзакции сохранения заказа.</returns>
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] OrderPost dto)
        {
            if (dto == null || dto.OrderProducts == null || dto.OrderProducts.Count == 0)
            {
                return BadRequest(new { message = "Данные заказа пусты или не содержат информации о товарах." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newOrder = new Order
                {
                    OrderID = dto.OrderID,
                    OrderDate = dto.OrderDate,
                    DeliveryDate = dto.DeliveryDate,
                    ClientID = dto.ClientID,
                    PickupCode = dto.PickupCode,
                    Status = dto.Status
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                foreach (var prodDto in dto.OrderProducts)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderID = (int)newOrder.OrderID,
                        ProductSKU = prodDto.ProductSKU,
                        Quantity = prodDto.Quantity
                    };

                    _context.OrderProducts.Add(orderProduct);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Заказ успешно сохранен в БД." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = $"Внутренняя ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}" });
            }
        }
    }
}