using System;
using System.Collections.Generic;

namespace DCIT318_Q3
{
    // Marker interface IInventoryItem
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    // GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    // Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // InventoryRepository<T>
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id); // will throw if not found
            item.Quantity = newQuantity;
        }
    }

    // WareHouseManager
    public class WareHouseManager
    {
        public InventoryRepository<ElectronicItem> _electronics = new();
        public InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            // Add 2-3 electronics
            try { _electronics.AddItem(new ElectronicItem(1, "Smartphone", 10, "BrandA", 24)); } catch { }
            try { _electronics.AddItem(new ElectronicItem(2, "Laptop", 5, "BrandB", 12)); } catch { }
            try { _electronics.AddItem(new ElectronicItem(3, "Headphones", 15, "BrandC", 6)); } catch { }

            // Add 2-3 groceries
            try { _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Now.AddMonths(12))); } catch { }
            try { _groceries.AddItem(new GroceryItem(102, "Milk Powder", 30, DateTime.Now.AddMonths(6))); } catch { }
            try { _groceries.AddItem(new GroceryItem(103, "Sugar", 40, DateTime.Now.AddMonths(18))); } catch { }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var items = repo.GetAllItems();
            foreach (var it in items)
            {
                Console.WriteLine($"ID: {it.Id}, Name: {it.Name}, Quantity: {it.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var manager = new WareHouseManager();

            // Seed data
            manager.SeedData();

            // Print groceries
            manager.PrintAllItems(manager._groceries);

            // Print electronics
            manager.PrintAllItems(manager._electronics);

            // Try to add a duplicate item (should be caught)
            try
            {
                manager._groceries.AddItem(new GroceryItem(101, "DuplicateRice", 10, DateTime.Now.AddMonths(6)));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Remove a non-existent item (caught and message shown)
            try
            {
                manager._electronics.RemoveItem(999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Update with invalid quantity (caught)
            try
            {
                manager._groceries.UpdateQuantity(102, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
