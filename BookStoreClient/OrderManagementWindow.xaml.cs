using BookStoreClient.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace BookStoreClient
{
    /// <summary>
    /// Окно управления заказами для сотрудников (Администратор / Менеджер).
    /// Обеспечивает поиск заказов по номеру, просмотр деталей, изменение даты доставки и статуса.
    /// </summary>
    public partial class OrderManagementWindow : Window
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _baseApiUrl = "http://localhost:5103/api/orders";
        private OrderDetailDto _currentOrder = null;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="OrderManagementWindow"/>.
        /// </summary>
        public OrderManagementWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string input = SearchOrderIdTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int orderId))
            {
                MessageBox.Show("Пожалуйста, введите корректный числовой номер заказа.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_baseApiUrl}/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    var options = jsonSerializerOptions;

                    _currentOrder = JsonSerializer.Deserialize<OrderDetailDto>(jsonResponse, options);

                    if (_currentOrder != null)
                    {
                        OrderDateTextBlock.Text = _currentOrder.OrderDate.ToShortDateString();
                        ClientFioTextBlock.Text = string.IsNullOrEmpty(_currentOrder.ClientFullName)
                            ? "Гость (Неавторизованный пользователь)"
                            : _currentOrder.ClientFullName;

                        DeliveryDatePicker.SelectedDate = _currentOrder.DeliveryDate;

                        SetStatusInComboBox(_currentOrder.Status);

                        DetailsGroupBox.Visibility = Visibility.Visible;
                        SaveButton.Visibility = Visibility.Visible;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Заказ с таким номером не найден.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    DetailsGroupBox.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении данных с сервера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOrder == null || DeliveryDatePicker.SelectedDate == null || StatusComboBox.SelectedItem == null) return;

            _currentOrder.DeliveryDate = DeliveryDatePicker.SelectedDate.Value;
            _currentOrder.Status = (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            try
            {
                string json = JsonSerializer.Serialize(_currentOrder);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"{_baseApiUrl}/{_currentOrder.OrderID}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные заказа успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить изменения на сервере.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetStatusInComboBox(string status)
        {
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == status)
                {
                    StatusComboBox.SelectedItem = item;
                    return;
                }
            }
            StatusComboBox.SelectedIndex = 0;
        }
    }
}