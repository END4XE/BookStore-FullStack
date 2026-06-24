using Final_Work.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_Work.Controllers
{
    /// <summary>
    /// Контроллер для управления данными о книгах в магазине.
    /// Предоставляет методы для получения списка книг и обновления информации о них.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BooksController"/> с внедрением контекста базы данных.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает полный список книг, доступных в базе данных.
        /// </summary>
        /// <returns>Список объектов книг со статусом успешного ответа.</returns>
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        /// <summary>
        /// Обновляет данные о книге по её артикулу (SKU).
        /// </summary>
        /// <param name="sku">Артикул книги, данные которой необходимо обновить.</param>
        /// <param name="updatedBook">Объект с обновленными параметрами книги.</param>
        /// <returns>Статус выполнения операции и информационное сообщение.</returns>
        [HttpPut("{sku}")]
        public IActionResult UpdateBook(string sku, [FromBody] Books updatedBook)
        {
            if (string.IsNullOrWhiteSpace(sku) || updatedBook == null || sku != updatedBook.SKU)
            {
                return BadRequest(new { message = "Некорректные данные товара или артикул" });
            }

            var existingBook = _context.Books.FirstOrDefault(b => b.SKU == sku);

            if (existingBook == null)
            {
                return NotFound(new { message = "Товар с таким артикулом не найден" });
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Unit = updatedBook.Unit;
            existingBook.Price = updatedBook.Price;
            existingBook.Author = updatedBook.Author;
            existingBook.Publisher = updatedBook.Publisher;
            existingBook.Category = updatedBook.Category;
            existingBook.Discount = updatedBook.Discount;
            existingBook.StockQuantity = updatedBook.StockQuantity;
            existingBook.Description = updatedBook.Description;
            existingBook.ImageName = updatedBook.ImageName;

            _context.SaveChanges();

            return Ok(new { message = "Товар успешно обновлен!" });
        }
    }
}