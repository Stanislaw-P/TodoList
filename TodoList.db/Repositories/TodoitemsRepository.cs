using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<List<TodoItem>?> TryGetForSelectedDateAsync(int userId, DateTime? selectedDate)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string sql;
            if (selectedDate.HasValue)
            {
                sql = @"
            SELECT id, user_id, title, description, is_completed, created_at, due_date
            FROM todoitems
            WHERE user_id = @UserId 
              AND DATE(due_date) = DATE(@SelectedDate)
              AND is_completed = FALSE";
            }
            else
            {
                sql = @"
            SELECT id, user_id, title, description, is_completed, created_at, due_date
            FROM todoitems
            WHERE user_id = @UserId 
              AND due_date IS NULL
              AND is_completed = FALSE";
            }

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            if (selectedDate.HasValue)
            {
                command.Parameters.AddWithValue("@SelectedDate", selectedDate.Value);
            }

            var items = new List<TodoItem>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new TodoItem
                {
                    Id = reader.GetInt32("id"),
                    UserId = reader.GetInt32("user_id"),
                    Title = reader.GetString("title"),
                    Description = await reader.IsDBNullAsync("description") ? null : reader.GetString("description"),
                    IsCompleted = reader.GetBoolean("is_completed"),
                    CreatedAt = reader.GetDateTime("created_at"),
                    DueDate = await reader.IsDBNullAsync("due_date") ? null : reader.GetDateTime("due_date")
                });
            }
            return items;
        }

        // В репозитории
        public async Task<List<DateTime>> GetDueDatesWithPendingTasksAsync(int userId)
        {
            using MySqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"
        SELECT DISTINCT DATE(due_date) 
        FROM todoitems 
        WHERE user_id = @UserId 
          AND due_date IS NOT NULL 
          AND is_completed = FALSE";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var dates = new List<DateTime>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dates.Add(reader.GetDateTime(0));
            }
            return dates;
        }
    }
}
