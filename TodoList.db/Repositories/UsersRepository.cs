using MySql.Data.MySqlClient;
using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        public UsersRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = "SELECT * FROM users";

            using var command = new MySqlCommand(sqlQuery, connection);
            using var reader = await command.ExecuteReaderAsync();

            var users = new List<User>();

            int idInd = reader.GetOrdinal("id");
            int nameInd = reader.GetOrdinal("name");
            int passHashInd = reader.GetOrdinal("password_hash");
            int emailInd = reader.GetOrdinal("email");
            int createdAtInd = reader.GetOrdinal("created_at");

            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(idInd),
                    Name = reader.GetString(nameInd),
                    PasswordHash = reader.GetString(passHashInd),
                    Email = reader.GetString(emailInd),
                    CreatedAt = reader.GetDateTime(createdAtInd)
                };
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Добавляет нового пользователя и возвращает его id
        /// </summary>
        /// <param name="user">Новый пользователь</param>
        /// <returns></returns>
        public async Task<int> AddAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"INSERT INTO users (name, password_hash, email)
                                VALUES (@Name, @PasswordHash, @Email);
                                SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Email", user.Email);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = "SELECT * FROM users WHERE email = @Email;";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Email", email);

            var reader = await command.ExecuteReaderAsync();

            int idInd = reader.GetOrdinal("id");
            int nameInd = reader.GetOrdinal("name");
            int passHashInd = reader.GetOrdinal("password_hash");
            int emailInd = reader.GetOrdinal("email");
            int createdAtInd = reader.GetOrdinal("created_at");

            await reader.ReadAsync();
            return new User
            {
                Id = reader.GetInt32(idInd),
                Name = reader.GetString(nameInd),
                PasswordHash = reader.GetString(passHashInd),
                Email = reader.GetString(emailInd),
                CreatedAt = reader.GetDateTime(createdAtInd)
            };
        }
    }
}
