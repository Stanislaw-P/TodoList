using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.db.Models;

namespace TodoList.db.Repositories
{
    public class TodoitemsRepository : ITodoitemsRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        public TodoitemsRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<TodoItem>> GetAllAsync()
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            string sqlQuery = "SELECT * FROM todoitems";

            using MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            using var reader = await command.ExecuteReaderAsync();

            var todoItems = new List<TodoItem>();

            while (await reader.ReadAsync())
            {
                var item = new TodoItem
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = await reader.IsDBNullAsync(3) ? null : reader.GetString(3),
                    IsCompleted = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                };
                todoItems.Add(item);
            }

            return todoItems;
        }
    }
}
