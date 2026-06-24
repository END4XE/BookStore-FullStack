using BookStoreClient.Models;
using Microsoft.Win32;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookStoreClient
{
    /// <summary>
    /// Окно корзины покупок. Обеспечивает просмотр текущего состава заказа, 
    /// удаление товаров, отправку данных на сервер и генерацию отчетного талона.
    /// </summary>
    public partial class CartWindow : Window
    {
        private static readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5103/api/")
        };

        private static readonly Random _random = new();

        private int _savedOrderId;
        private DateTime _savedOrderDate;
        private int _savedPickupCode;
        private List<OrderItem> _savedOrderItems = [];

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CartWindow"/>.
        /// </summary>
        public CartWindow()
        {
            InitializeComponent();

            if (OrderManager.CurrentClientID.HasValue && !string.IsNullOrEmpty(OrderManager.CurrentClientFIO))
            {
                ClientFioTextBlock.Text = OrderManager.CurrentClientFIO;
            }
            else
            {
                ClientFioTextBlock.Text = "Гость";
            }

            RefreshCart();
        }

        private void RefreshCart()
        {
            OrderItemsListBox.ItemsSource = null;
            OrderItemsListBox.ItemsSource = OrderManager.CurrentOrder;
            TotalPriceTextBlock.Text = OrderManager.GetOrderTotal()?.ToString("N2") ?? "0.00";
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string bookSKU)
            {
                OrderManager.RemoveProduct(bookSKU);
                RefreshCart();

                if (OrderManager.CurrentOrder.Count == 0)
                {
                    MessageBox.Show("Ваша корзина теперь пуста.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private async void SubmitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderManager.CurrentOrder.Count == 0)
            {
                MessageBox.Show("Нельзя сформировать пустой заказ!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int newOrderId = 1;

                var responseMax = await _httpClient.GetAsync("Orders/max-id");
                if (responseMax.IsSuccessStatusCode)
                {
                    newOrderId = await responseMax.Content.ReadFromJsonAsync<int>() + 1;
                }

                var newOrderDto = new OrderPost
                {
                    OrderID = newOrderId,
                    OrderDate = DateTime.Now,
                    DeliveryDate = DateTime.Now.AddDays(3),
                    ClientID = OrderManager.CurrentClientID,
                    PickupCode = _random.Next(100, 1000),
                    Status = "Новый"
                };

                foreach (var item in OrderManager.CurrentOrder)
                {
                    newOrderDto.OrderProducts.Add(new OrderProduct
                    {
                        ProductSKU = item.Product.SKU,
                        Quantity = item.Quantity
                    });
                }

                var responsePost = await _httpClient.PostAsJsonAsync("Orders", newOrderDto);

                if (responsePost.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Заказ №{newOrderId} успешно сформирован и сохранен в БД!\nТеперь вы можете сохранить талон заказа.",
                                    "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

                    _savedOrderId = newOrderId;
                    _savedOrderDate = newOrderDto.OrderDate;
                    _savedPickupCode = newOrderDto.PickupCode;
                    _savedOrderItems = [.. OrderManager.CurrentOrder];

                    SubmitOrderButton.Visibility = Visibility.Collapsed;
                    SaveTicketButton.Visibility = Visibility.Visible;
                }
                else
                {
                    string error = await responsePost.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера при сохранении заказа: {error}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось связаться с сервером API. Проверьте подключение.\nДетали: {ex.Message}",
                                "Ошибка сети", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveTicketButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ticketBuilder = new StringBuilder();
                ticketBuilder.AppendLine("========================================");
                ticketBuilder.AppendLine($"       ТАЛОН ЗАКАЗА №{_savedOrderId}");
                ticketBuilder.AppendLine("========================================");
                ticketBuilder.AppendLine($"Дата заказа: {_savedOrderDate:dd.MM.yyyy HH:mm}");
                ticketBuilder.AppendLine($"Код получения: {_savedPickupCode}");
                ticketBuilder.AppendLine("----------------------------------------");
                ticketBuilder.AppendLine("Состав заказа:");

                decimal? totalSum = 0;
                foreach (var item in _savedOrderItems)
                {
                    ticketBuilder.AppendLine($"- {item.Product.Title} | Кол-во: {item.Quantity} шт. | Цена: {item.Product.Price} руб.");
                    totalSum += item.Quantity * item.Product.Price;
                }
                ticketBuilder.AppendLine("----------------------------------------");
                ticketBuilder.AppendLine($"ИТОГО К ОПЛАТЕ: {totalSum?.ToString("N2")} руб.");
                ticketBuilder.AppendLine("========================================");
                ticketBuilder.AppendLine("Спасибо за покупку!");

                var saveFileDialog = new SaveFileDialog
                {
                    FileName = $"Талон_Заказа_{_savedOrderId}",
                    DefaultExt = ".txt",
                    Filter = "Текстовые файлы (*.txt)|*.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, ticketBuilder.ToString());

                    MessageBox.Show("Талон успешно сохранен на вашем устройстве!", "Файл сохранен", MessageBoxButton.OK, MessageBoxImage.Information);

                    OrderManager.CurrentOrder.Clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при записи файла: {ex.Message}", "Ошибка экспорта", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}