public class InventoryController
{
    private readonly InventoryModel _model;

    public event Action<string>? EventLogged;

    public InventoryController(InventoryModel model)
    {
        _model = model;
        _model.ItemAdded += OnItemAdded;
        _model.ItemUpdated += OnItemUpdated;
    }

    private void OnItemAdded(object sender, Item item)
    {
        string message = $"[EVENTO] New item added: {item.Name}";
        Console.WriteLine(message);
        EventLogged?.Invoke(message);
    }

    private void OnItemUpdated(object sender, Item item)
    {
        string message = $"[EVENTO] Quantity of item updated: {item.Name} ({item.Quantity})";
        Console.WriteLine(message);
        EventLogged?.Invoke(message);
    }

    public List<Item> GetInventory() => _model.GetAllItems();

    public Item? GetItemById(string id) => _model.GetItemById(id);

    public bool AddNewItem(string name, string description, int quantity, decimal price, string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Error: Item name cannot be empty.");
                return false;
            }
            if (quantity < 0)
            {
                Console.WriteLine("Error: Item quantity cannot be negative.");
                return false;
            }
            if (price < 0)
            {
                Console.WriteLine("Error: Item price cannot be negative.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                Console.WriteLine("Error: Item category cannot be empty.");
                return false;
            }
            if (_model.GetAllItems().Any(i => string.Equals(i.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Item with the same name already exists.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Error: Item description cannot be empty.");
                return false;
            }

            var item = new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = name.Trim(),
                Description = description.Trim(),
                Quantity = quantity,
                Price = price,
                Category = category.Trim()
            };

            _model.AddItem(item);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateItemQuantity(string id, int newQuantity)
    {
        if (string.IsNullOrWhiteSpace(id) || newQuantity < 0)
        {
            return false;
        }

        try
        {
            var item = _model.GetItemById(id);
            if (item != null)
            {
                item.Quantity = newQuantity;
                _model.UpdateItem(item);
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool RemoveItem(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        try
        {
            return _model.DeleteItem(id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<Item> SearchInventory(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return _model.GetAllItems();
        }

        return _model.GetAllItems()
            .Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        i.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        i.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Item> GetLowStockAlert(int threshold = 5)
    {
        return _model.GetLowStockItems(threshold);
    }
}
