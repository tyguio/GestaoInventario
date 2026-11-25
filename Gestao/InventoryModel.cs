using System.Text.Json;

// Represents an inventory item
public class Item
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

// Handles inventory data operations and JSON persistence
public class InventoryModel
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private List<Item> _items = new();
    private readonly string _dataFilePath;

    public delegate void ItemAddedEventHandler(object sender, Item item);
    public event ItemAddedEventHandler? ItemAdded;
    public event ItemAddedEventHandler? ItemUpdated;

    public InventoryModel(string dataFilePath)
    {
        _dataFilePath = dataFilePath;
        LoadInventory();
    }

    public void LoadInventory()
    {
        if (!File.Exists(_dataFilePath))
        {
            _items = new List<Item>();
            return;
        }

        try
        {
            string json = File.ReadAllText(_dataFilePath);
            _items = JsonSerializer.Deserialize<List<Item>>(json) ?? new List<Item>();
        }
        catch (Exception ex) when (ex is JsonException || ex is IOException)
        {
            Console.WriteLine("Error loading inventory: Unable to load inventory data. The file may be corrupted.");
            _items = new List<Item>();
        }
    }

    public void SaveInventory()
    {
        string? directory = Path.GetDirectoryName(_dataFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(_items, JsonOptions);
        File.WriteAllText(_dataFilePath, json);
    }

    public List<Item> GetAllItems() => _items;

    public Item? GetItemById(string id) => _items.FirstOrDefault(i => i.Id == id);

    private static void ValidateItem(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Item name cannot be empty.");

        if (item.Quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(item.Quantity), "Item quantity cannot be negative.");

        if (item.Price < 0)
            throw new ArgumentOutOfRangeException(nameof(item.Price), "Item price cannot be negative.");

        if (string.IsNullOrWhiteSpace(item.Category))
            throw new ArgumentException("Item category cannot be empty.");

        if (string.IsNullOrWhiteSpace(item.Description))
            throw new ArgumentException("Item description cannot be empty.");
    }

    public void AddItem(Item item)
    {
        ValidateItem(item);

        if (_items.Any(i => i.Id == item.Id))
            throw new ArgumentException("Item ID must be unique.");

        item.LastUpdated = DateTime.Now;
        _items.Add(item);
        SaveInventory();
        ItemAdded?.Invoke(this, item);
    }

    public void UpdateItem(Item item)
    {
        ValidateItem(item);

        var existingItem = GetItemById(item.Id);
        if (existingItem != null)
        {
            int index = _items.IndexOf(existingItem);
            item.LastUpdated = DateTime.Now;
            _items[index] = item;
            SaveInventory();
            ItemUpdated?.Invoke(this, item);
        }
    }

    public bool DeleteItem(string id)
    {
        var item = GetItemById(id);
        if (item == null)
        {
            return false;
        }

        _items.Remove(item);
        SaveInventory();
        return true;
    }

    public List<Item> GetItemsByCategory(string category)
    {
        return _items
            .Where(i => string.Equals(i.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Item> GetLowStockItems(int threshold)
    {
        return _items.Where(i => i.Quantity < threshold).ToList();
    }
}
