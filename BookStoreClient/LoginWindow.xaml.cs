using BookStoreClient.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace BookStoreClient
{
    /// <summary>
    /// Окно авторизации пользователей. Обеспечивает аутентификацию
    /// клиентов и сотрудников магазина через удаленный сервер API.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new();
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LoginWindow"/> с привязкой к главному окну.
        /// </summary>
        /// <param name="mainWindow">Экземпляр главного окна приложения для управления доступными функциями.</param>
        public LoginWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordTextBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string apiUrl = "http://localhost:5103/api/auth/login";

                var loginData = new { Login = login, Password = password };
                string jsonBody = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;

                        int roleId = root.GetProperty("roleId").GetInt32();
                        string roleName = roleId switch
                        {
                            1 => "Администратор",
                            2 => "Менеджер",
                            3 => "Авторизованный клиент",
                            _ => "Неизвестная роль"
                        };

                        if (root.TryGetProperty("userID", out JsonElement idProp) ||
                            root.TryGetProperty("userId", out idProp) ||
                            root.TryGetProperty("UserID", out idProp))
                        {
                            OrderManager.CurrentClientID = idProp.GetInt32();
                        }

                        if (root.TryGetProperty("fullName", out JsonElement nameProp) ||
                            root.TryGetProperty("FullName", out nameProp))
                        {
                            OrderManager.CurrentClientFIO = nameProp.GetString();
                        }
                        else
                        {
                            OrderManager.CurrentClientFIO = "Авторизованный пользователь";
                        }

                        MessageBox.Show($"Добро пожаловать, {OrderManager.CurrentClientFIO}!\nРоль: {roleName}", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                        _mainWindow.EnableAdminFeatures(roleName);
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу API: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}