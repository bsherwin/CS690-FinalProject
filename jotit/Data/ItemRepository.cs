using Microsoft.Data.Sqlite;
using JotIt.Models;

namespace JotIt.Data;

public class ItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Body TEXT NOT NULL,
                Category TEXT NOT NULL DEFAULT '',
                DueDate TEXT,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
            )";
        command.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Add(Item item)
    {
        using var connection = Open();
        var command = connection.CreateCommand();

        if (item is TaskItem task)
        {
            command.CommandText = "INSERT INTO Items (Body, Category, DueDate) VALUES ($body, $category, $dueDate)";
            command.Parameters.AddWithValue("$body", task.Body);
            command.Parameters.AddWithValue("$category", task.Category);
            command.Parameters.AddWithValue("$dueDate", task.DueDate);
        }
        else
        {
            command.CommandText = "INSERT INTO Items (Body, Category) VALUES ($body, $category)";
            command.Parameters.AddWithValue("$body", item.Body);
            command.Parameters.AddWithValue("$category", item.Category);
        }

        command.ExecuteNonQuery();
    }

    public List<NoteItem> GetNotes()
    {
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Body, Category FROM Items WHERE DueDate IS NULL 
            ORDER BY CASE Category 
                WHEN 'Work' THEN 1 
                WHEN 'Home' THEN 2 
                WHEN 'Personal' THEN 3 
                ELSE 4 
            END";

        using var reader = command.ExecuteReader();
        var notes = new List<NoteItem>();
        while (reader.Read())
        {
            notes.Add(new NoteItem
            {
                Id = (long)reader["Id"],
                Body = (string)reader["Body"],
                Category = reader["Category"] == DBNull.Value ? "" : (string)reader["Category"]
            });
        }
        return notes;
    }

    public List<TaskItem> GetTasks()
    {
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Body, Category, DueDate FROM Items WHERE DueDate IS NOT NULL 
            ORDER BY CASE Category 
                WHEN 'Work' THEN 1 
                WHEN 'Home' THEN 2 
                WHEN 'Personal' THEN 3 
                ELSE 4 
            END, DueDate ASC";

        using var reader = command.ExecuteReader();
        var tasks = new List<TaskItem>();
        while (reader.Read())
        {
            tasks.Add(new TaskItem
            {
                Id = (long)reader["Id"],
                Body = (string)reader["Body"],
                Category = reader["Category"] == DBNull.Value ? "" : (string)reader["Category"],
                DueDate = (string)reader["DueDate"]
            });
        }
        return tasks;
    }

    public Item? GetById(long id)
    {
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Body, Category, DueDate FROM Items WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        long itemId = (long)reader["Id"];
        string body = (string)reader["Body"];
        string category = reader["Category"] == DBNull.Value ? "" : (string)reader["Category"];

        if (reader["DueDate"] == DBNull.Value)
            return new NoteItem { Id = itemId, Body = body, Category = category };

        return new TaskItem { Id = itemId, Body = body, Category = category, DueDate = (string)reader["DueDate"] };
    }

    public bool SetDueDate(long id, string? dueDate)
    {
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Items SET DueDate = $dueDate WHERE Id = $id";
        command.Parameters.AddWithValue("$dueDate", dueDate ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(long id)
    {
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Items WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteNonQuery() > 0;
    }
}
