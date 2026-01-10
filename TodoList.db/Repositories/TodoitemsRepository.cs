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

        public async Task<List<TodoItem>> GetAllAsync(int userId)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"SELECT title, description, is_completed, created_at
                                FROM todoitems
                                WHERE user_id = @UserId;";

            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();

            var todoItems = new List<TodoItem>();

            while (await reader.ReadAsync())
            {
                todoItems.Add(new TodoItem
                {
                    Title = reader.GetString(0),
                    Description = await reader.IsDBNullAsync(1) ? null : reader.GetString(1),
                    IsCompleted = reader.GetBoolean(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return todoItems;
        }

        public async Task AddAsync(TodoItem newTodoItem)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"INSERT INTO todoitems (title, description, user_id, created_at)
                                VALUES (@Title, @Description, @UserId, @CreatedAt);";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Title", newTodoItem.Title);
            command.Parameters.AddWithValue("@Description", newTodoItem.Description);
            command.Parameters.AddWithValue("@UserId", newTodoItem.UserId);
            command.Parameters.AddWithValue("@CreatedAt", newTodoItem.CreatedAt);

            await command.ExecuteNonQueryAsync();
        }
    }
}
