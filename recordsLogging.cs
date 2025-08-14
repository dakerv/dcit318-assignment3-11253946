using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DCIT318_Q5
{
    // IInventoryEntity marker
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable InventoryItem record implementing IInventoryEntity
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic InventoryLogger<T>
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
                {
                    JsonSerializer.Serialize(stream, _log);
                }
            }
            catch
            {
                throw;
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _log = new List<T>();
                    return;
                }

                using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    var items = JsonSerializer.Deserialize<List<T>>(stream);
                    _log = items ?? new List<T>();
                }
            }
            catch
            {
                throw;
            }
        }
    }

    // InventoryApp
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "ItemA", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "ItemB", 5, DateTime.Now));
            _logger.Add(new InventoryItem(3, "ItemC", 20, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            foreach (var it in items)
            {
                Console.WriteLine($"ID: {it.Id}, Name: {it.Name}, Quantity: {it.Quantity}, DateAdded: {it.DateAdded}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string filePath = "inventory_log.json";

            var app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();

            // Clear memory (simulate new session) by creating a new app instance
            var newApp = new InventoryApp(filePath);
            newApp.LoadData();
            newApp.PrintAllItems();
        }
    }
}
