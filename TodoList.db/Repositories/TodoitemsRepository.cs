using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
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

        public async Task<List<TodoItem>> GetAllAsync(int userId)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"SELECT id, user_id, title, description, is_completed, created_at
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
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = await reader.IsDBNullAsync(3) ? null : reader.GetString(3),
                    IsCompleted = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
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

        public async Task ToggleStatusAsync(int todoItemId)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"UPDATE todoitems
                                SET is_completed = NOT is_completed
                                WHERE id = @TodoItemId;";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@TodoItemId", todoItemId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int TodoItemId)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = "DELETE FROM todoitems WHERE id = @TodoItemId;";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@TodoItemId", TodoItemId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(TodoItem editedTodoItem)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"UPDATE todoitems
                                SET title = @Title, description = @Description
                                WHERE id = @Id;";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue(@"Title", editedTodoItem.Title);
            command.Parameters.AddWithValue(@"Description", editedTodoItem.Description);
            command.Parameters.AddWithValue(@"Id", editedTodoItem.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<TodoItem?> TryGetByIdAsync(int id)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sqlQuery = @"SELECT id, user_id, title, description, is_completed, created_at
                                FROM todoitems
                                WHERE id = @Id";
            using var command = new MySqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue(@"Id", id);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                return new TodoItem
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = await reader.IsDBNullAsync(3) ? null : reader.GetString(3),
                    IsCompleted = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }

    }
}
