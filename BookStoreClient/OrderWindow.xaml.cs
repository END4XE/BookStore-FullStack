using BookStoreClient.Models;
using System.Windows;

namespace BookStoreClient
{
    /// <summary>
    /// Логика взаимодействия для окна подтверждения добавления товара OrderWindow.xaml.
    /// </summary>
    public partial class OrderWindow : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="OrderWindow"/> и заполняет данные о выбранной книге.
        /// </summary>
        /// <param name="selectedBook">Объект выбранной пользователем книги.</param>
        public OrderWindow(Books selectedBook)
        {
            InitializeComponent();

            TitleTextBlock.Text = selectedBook.Title;
            AuthorTextBlock.Text = !string.IsNullOrEmpty(selectedBook.Author) ? selectedBook.Author : "Не указан";
            PriceTextBlock.Text = selectedBook.Price?.ToString("N2") ?? "0.00";
        }

        /// <summary>
        /// Обработчик нажатия кнопки закрытия окна.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
