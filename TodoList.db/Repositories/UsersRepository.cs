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

            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Email = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                };
                users.Add(user);
            }

            return users;
        }
    }
}
