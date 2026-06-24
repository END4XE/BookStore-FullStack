using BookStoreClient.Models;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace BookStoreClient
{
    /// <summary>
    /// Главное окно приложения, предоставляющее интерфейс просмотра, 
    /// фильтрации книг и перехода к оформлению заказов.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new();
        private List<Books> _allBooks = [];

        /// <summary>
        /// Получает или задает текущую роль пользователя в системе. По умолчанию — "Гость".
        /// </summary>
        public string UserRole { get; set; } = "Гость";

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LoadBooks();
        }

        private async void LoadBooks()
        {
            try
            {
                string apiUrl = "http://localhost:5103/api/books";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    var options = jsonSerializerOptions;
                    _allBooks = JsonSerializer.Deserialize<List<Books>>(jsonResponse, options) ?? [];

                    FillManufacturerFilter();
                    ApplyFilters();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillManufacturerFilter()
        {
            if (ManufacturerComboBox == null || _allBooks == null) return;

            var manufacturers = _allBooks
                .Select(b => b.Publisher)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            manufacturers.Insert(0, "Все производители");

            ManufacturerComboBox.ItemsSource = manufacturers;
            ManufacturerComboBox.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            if (BooksListBox == null || SortCriterionComboBox == null || SortDirectionComboBox == null ||
                SearchTextBox == null || ManufacturerComboBox == null || CountTextBlock == null ||
                MinPriceTextBox == null || MaxPriceTextBox == null || _allBooks == null) return;

            var filtered = _allBooks.AsEnumerable();

            string searchText = SearchTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(b => b.Title != null && b.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));
            }

            if (decimal.TryParse(MinPriceTextBox.Text.Trim(), out decimal minPrice))
            {
                filtered = filtered.Where(b => b.Price >= minPrice);
            }

            if (decimal.TryParse(MaxPriceTextBox.Text.Trim(), out decimal maxPrice))
            {
                filtered = filtered.Where(b => b.Price <= maxPrice);
            }

            if (ManufacturerComboBox.SelectedIndex > 0)
            {
                string selectedManufacturer = ManufacturerComboBox.SelectedItem.ToString();
                filtered = filtered.Where(b => b.Publisher == selectedManufacturer);
            }

            bool isAscending = SortDirectionComboBox.SelectedIndex == 0;
            if (SortCriterionComboBox.SelectedIndex == 1)
            {
                filtered = isAscending ? filtered.OrderBy(b => b.Price) : filtered.OrderByDescending(b => b.Price);
            }
            else if (SortCriterionComboBox.SelectedIndex == 2)
            {
                filtered = isAscending ? filtered.OrderBy(b => b.Title) : filtered.OrderByDescending(b => b.Title);
            }

            var resultList = filtered.ToList();

            CountTextBlock.Text = $" Выведено: {resultList.Count} из {_allBooks.Count}";
            BooksListBox.ItemsSource = resultList;

            if (NoResultsTextBlock != null)
            {
                NoResultsTextBlock.Visibility = resultList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void FilterChanged(object sender, EventArgs e) => ApplyFilters();

        private void LoginMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginMenuButton.Content.ToString() == "Выйти")
            {
                EnableAdminFeatures("Гость");

                OrderManager.CurrentClientID = null;
                OrderManager.CurrentClientFIO = "Гость";

                MessageBox.Show("Вы успешно вышли из аккаунта.", "Выход", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            LoginWindow loginWindow = new(this)
            {
                Owner = this
            };
            loginWindow.ShowDialog();
        }

        private void AdminOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow managementWindow = new()
            {
                Owner = this
            };
            managementWindow.ShowDialog();
        }

        /// <summary>
        /// Активирует элементы интерфейса управления для сотрудников с правами администратора или менеджера.
        /// </summary>
        /// <param name="role">Название роли пользователя, вошедшего в систему.</param>
        public void EnableAdminFeatures(string role)
        {
            UserRole = role;

            if (role == "Администратор" || role == "Менеджер")
            {
                LoginMenuButton.Content = "Выйти";
                AdminOrdersButton.Visibility = Visibility.Visible;
            }
            else
            {
                AdminOrdersButton.Visibility = Visibility.Collapsed;

                if (role == "Гость")
                {
                    LoginMenuButton.Content = "Войти";
                }
                else
                {
                    LoginMenuButton.Content = "Выйти";
                }
            }
        }

        private void UpdateOrderButtonState()
        {
            int totalItems = OrderManager.GetTotalItemsCount();

            if (totalItems > 0)
            {
                ViewOrderButton.Content = $"🛒 Просмотр заказа ({totalItems})";
                ViewOrderButton.Visibility = Visibility.Visible;
            }
            else
            {
                ViewOrderButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку «Заказать».
        /// Добавляет товар в корзину, открывает информационное окно и обновляет видимость кнопки просмотра заказа.
        /// </summary>
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.DataContext is Books selectedBook)
            {
                OrderManager.AddProduct(selectedBook);

                OrderWindow orderWindow = new(selectedBook)
                {
                    Owner = this
                };
                orderWindow.ShowDialog();

                ViewOrderButton.Visibility = Visibility.Visible;
                ViewOrderButton.Content = $"🛒 Просмотр заказа ({OrderManager.GetTotalItemsCount()})";
            }
        }

        private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new()
            {
                Owner = this
            };
            cartWindow.ShowDialog();

            UpdateOrderButtonState();
        }
    }
}