using System.IO;
using Newtonsoft.Json;

namespace DualDolmen
{
    public class UsersApp
    {
        private readonly string _usersFilePath; // Путь к файлу с данными пользователей
        private Dictionary<string, string> _users = new Dictionary<string, string>(); // Словарь для хранения пользователей (логин-пароль)

        public UsersApp(string usersFilePath)
        {
            _usersFilePath = usersFilePath ?? throw new ArgumentNullException(nameof(usersFilePath)); // Проверка на null и инициализация пути к файлу
            LoadUsers(); // Загрузка пользователей при создании экземпляра класса
        }

        private void LoadUsers() // Метод загрузки пользователей из файла
        {
            try
            { // Проверка существования файла и что он не пустой
                if (File.Exists(_usersFilePath) && new FileInfo(_usersFilePath).Length > 0)
                {
                    string json = File.ReadAllText(_usersFilePath); // Чтение JSON из файла
                    var loadedUsers = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); // Десериализация JSON в словарь
                    _users = loadedUsers ?? new Dictionary<string, string>(); // Инициализация словаря пользователей
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователей: {ex.Message}");
                _users = new Dictionary<string, string>();
            }
        }

        private void SaveUsers()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_users, Formatting.Indented); // Сериализация словаря в JSON с отступами
                File.WriteAllText(_usersFilePath, json); // Запись JSON в файл
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения пользователей: {ex.Message}");
                throw;
            }
        }

        public void RegisterUser(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Имя пользователя не может быть пустым!");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым!");

            if (string.IsNullOrWhiteSpace(confirmPassword))
                throw new ArgumentException("Подтверждение пароля не может быть пустым!");

            if (password != confirmPassword)
                throw new ArgumentException("Пароли не совпадают!");

            if (_users.ContainsKey(username))
                throw new InvalidOperationException("Пользователь с таким именем уже существует!");

            _users[username] = password ?? throw new ArgumentNullException(nameof(password)); // Добавление нового пользователя
            SaveUsers();  // Сохранение изменений
        }

        public bool Authenticate(string username, string password) // Метод аутентификации пользователя
        {
            if (username == null || password == null) // Проверка на null
                return false;
            // Проверка существования пользователя и совпадения пароля
            return _users.TryGetValue(username, out string? storedPassword) && storedPassword == password;
        }

        public bool DeleteUser(string username) // Метод удаления пользователя
        {
            if (username == null) // Проверка на null
                return false;

            if (_users.ContainsKey(username)) // Если пользователь существует
            {
                _users.Remove(username);  // Удаление пользователя
                SaveUsers(); // Сохранение изменений
                return true;
            }
            return false;
        }


        public bool UserExists(string username) // Метод проверки существования пользователя
        {
            return username != null && _users.ContainsKey(username); // Проверка на null и существование в словаре
        }
    }
}