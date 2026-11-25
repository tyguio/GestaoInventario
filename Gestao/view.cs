using System.Drawing;
using System.Windows.Forms;

public interface IInventoryView
{
    void DisplayInventory(List<Item> items);
    void DisplayItemDetails(Item item);
    void ShowMessage(string message);
    void RefreshView();
}

public class InventoryFormView : Form, IInventoryView
{
    private readonly InventoryController _controller;

    private DataGridView _itemsGrid = null!;
    private TextBox idTextBox = null!;
    private TextBox nameTextBox = null!;
    private TextBox descriptionTextBox = null!;
    private NumericUpDown quantityNumeric = null!;
    private NumericUpDown priceNumeric = null!;
    private ComboBox categoryComboBox = null!;
    private TextBox searchTextBox = null!;
    private NumericUpDown thresholdNumeric = null!;
    private Button addButton = null!;
    private Button updateButton = null!;
    private Button deleteButton = null!;
    private Button searchButton = null!;
    private Button lowStockButton = null!;
    private Button refreshButton = null!;
    private Button clearButton = null!;
    private Label statusLabel = null!;

    public InventoryFormView(InventoryController controller)
    {
        _controller = controller;
        InitializeComponents();
        _controller.EventLogged += OnControllerEventLogged;
        RefreshView();
    }

    private void OnControllerEventLogged(string message)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(() => statusLabel.Text = message);
        }
        else
        {
            statusLabel.Text = message;
        }
    }

    private void InitializeComponents()
    {
        Text = "Gestão de Inventário";
        Size = new Size(960, 620);
        MinimumSize = new Size(860, 540);
        StartPosition = FormStartPosition.CenterScreen;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(10)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Top search and filter bar
        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            Padding = new Padding(0, 0, 0, 8)
        };

        searchTextBox = new TextBox { Width = 220, PlaceholderText = "Pesquisar nome, descrição ou categoria..." };
        searchButton = new Button { Text = "Pesquisar", AutoSize = true };
        var thresholdLabel = new Label { Text = "Limite Stock Baixo:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 6, 0, 0) };
        thresholdNumeric = new NumericUpDown { Width = 65, Minimum = 1, Maximum = 10000, Value = 5 };
        lowStockButton = new Button { Text = "Stock Baixo", AutoSize = true };
        refreshButton = new Button { Text = "Mostrar Todos", AutoSize = true };

        topPanel.Controls.Add(searchTextBox);
        topPanel.Controls.Add(searchButton);
        topPanel.Controls.Add(thresholdLabel);
        topPanel.Controls.Add(thresholdNumeric);
        topPanel.Controls.Add(lowStockButton);
        topPanel.Controls.Add(refreshButton);

        // Inventory grid
        _itemsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        // Form panel
        var formGroup = new GroupBox
        {
            Text = "Detalhes do Artigo",
            Dock = DockStyle.Fill,
            AutoSize = true,
            Padding = new Padding(10)
        };

        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 4
        };
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        idTextBox = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };
        nameTextBox = new TextBox { Dock = DockStyle.Fill };
        categoryComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown };
        categoryComboBox.Items.AddRange(new object[] { "Eletrónica", "Ferramentas", "Consumíveis", "Escritório", "Outros" });
        quantityNumeric = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 0, Maximum = 1000000 };
        priceNumeric = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 0, Maximum = 1000000, DecimalPlaces = 2 };
        descriptionTextBox = new TextBox { Dock = DockStyle.Fill };

        formLayout.Controls.Add(new Label { Text = "ID:", AutoSize = true, Padding = new Padding(0, 6, 6, 0) }, 0, 0);
        formLayout.Controls.Add(idTextBox, 1, 0);
        formLayout.Controls.Add(new Label { Text = "Nome:", AutoSize = true, Padding = new Padding(10, 6, 6, 0) }, 2, 0);
        formLayout.Controls.Add(nameTextBox, 3, 0);

        formLayout.Controls.Add(new Label { Text = "Categoria:", AutoSize = true, Padding = new Padding(0, 6, 6, 0) }, 0, 1);
        formLayout.Controls.Add(categoryComboBox, 1, 1);
        formLayout.Controls.Add(new Label { Text = "Quantidade:", AutoSize = true, Padding = new Padding(10, 6, 6, 0) }, 2, 1);
        formLayout.Controls.Add(quantityNumeric, 3, 1);

        formLayout.Controls.Add(new Label { Text = "Preço (€):", AutoSize = true, Padding = new Padding(0, 6, 6, 0) }, 0, 2);
        formLayout.Controls.Add(priceNumeric, 1, 2);
        formLayout.Controls.Add(new Label { Text = "Descrição:", AutoSize = true, Padding = new Padding(10, 6, 6, 0) }, 2, 2);
        formLayout.Controls.Add(descriptionTextBox, 3, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };

        clearButton = new Button { Text = "Limpar", AutoSize = true };
        deleteButton = new Button { Text = "Remover", AutoSize = true };
        updateButton = new Button { Text = "Atualizar Qtd", AutoSize = true };
        addButton = new Button { Text = "Adicionar", AutoSize = true };

        buttonPanel.Controls.Add(clearButton);
        buttonPanel.Controls.Add(deleteButton);
        buttonPanel.Controls.Add(updateButton);
        buttonPanel.Controls.Add(addButton);

        formLayout.Controls.Add(buttonPanel, 0, 3);
        formLayout.SetColumnSpan(buttonPanel, 4);
        formGroup.Controls.Add(formLayout);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            Text = "Pronto.",
            Padding = new Padding(0, 6, 0, 0)
        };

        mainLayout.Controls.Add(topPanel, 0, 0);
        mainLayout.Controls.Add(_itemsGrid, 0, 1);
        mainLayout.Controls.Add(formGroup, 0, 2);
        mainLayout.Controls.Add(statusLabel, 0, 3);
        Controls.Add(mainLayout);

        // Attach event handlers
        addButton.Click += AddButton_Click;
        updateButton.Click += UpdateButton_Click;
        deleteButton.Click += DeleteButton_Click;
        searchButton.Click += SearchButton_Click;
        lowStockButton.Click += LowStockButton_Click;
        refreshButton.Click += (_, _) => RefreshView();
        clearButton.Click += (_, _) => ClearInputs();
        _itemsGrid.SelectionChanged += ItemsGrid_SelectionChanged;
    }

    public void DisplayInventory(List<Item> items)
    {
        _itemsGrid.DataSource = null;
        _itemsGrid.DataSource = items.ToList();
    }

    public void DisplayItemDetails(Item item)
    {
        idTextBox.Text = item.Id;
        nameTextBox.Text = item.Name;
        descriptionTextBox.Text = item.Description;
        categoryComboBox.Text = item.Category;
        quantityNumeric.Value = Math.Clamp(item.Quantity, quantityNumeric.Minimum, quantityNumeric.Maximum);
        priceNumeric.Value = Math.Clamp(item.Price, priceNumeric.Minimum, priceNumeric.Maximum);
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Gestão de Inventário", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void RefreshView()
    {
        DisplayInventory(_controller.GetInventory());
    }

    private void ClearInputs()
    {
        idTextBox.Clear();
        nameTextBox.Clear();
        descriptionTextBox.Clear();
        categoryComboBox.Text = string.Empty;
        quantityNumeric.Value = 0;
        priceNumeric.Value = 0;
    }

    private void ItemsGrid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_itemsGrid.SelectedRows.Count > 0 && _itemsGrid.SelectedRows[0].DataBoundItem is Item selectedItem)
        {
            DisplayItemDetails(selectedItem);
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            ShowMessage("Item name cannot be empty.");
            return false;
        }
        if (quantityNumeric.Value < 0)
        {
            ShowMessage("Item quantity cannot be negative.");
            return false;
        }
        if (priceNumeric.Value < 0)
        {
            ShowMessage("Item price cannot be negative.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(categoryComboBox.Text))
        {
            ShowMessage("Item category cannot be empty.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
        {
            ShowMessage("Item description cannot be empty.");
            return false;
        }
        return true;
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateInputs())
        {
            return;
        }

        string name = nameTextBox.Text;
        string description = descriptionTextBox.Text;
        int quantity = (int)quantityNumeric.Value;
        decimal price = priceNumeric.Value;
        string category = categoryComboBox.Text;

        try
        {
            if (_controller.AddNewItem(name, description, quantity, price, category))
            {
                ShowMessage("Item added successfully");
                ClearInputs();
                RefreshView();
            }
            else
            {
                ShowMessage("Error adding item (verifique se já existe um artigo com o mesmo nome).");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Error adding item: {ex.Message}");
        }
    }

    private void SearchButton_Click(object? sender, EventArgs e)
    {
        string query = searchTextBox.Text.Trim();
        List<Item> searchResults = _controller.SearchInventory(query);

        DisplayInventory(searchResults);
        if (searchResults.Count > 0)
        {
            ShowMessage($"Search completed. {searchResults.Count} item(s) found.");
        }
        else
        {
            ShowMessage("No items found matching the search criteria.");
        }
    }

    private void LowStockButton_Click(object? sender, EventArgs e)
    {
        int threshold = (int)thresholdNumeric.Value;
        List<Item> lowStockItems = _controller.GetLowStockAlert(threshold);

        DisplayInventory(lowStockItems);
        if (lowStockItems.Count > 0)
        {
            ShowMessage($"Low stock alert: {lowStockItems.Count} item(s) found.");
        }
        else
        {
            ShowMessage("No low stock items found.");
        }
    }

    private void UpdateButton_Click(object? sender, EventArgs e)
    {
        string id = idTextBox.Text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            ShowMessage("Please select an item to update.");
            return;
        }

        int newQuantity = (int)quantityNumeric.Value;
        if (_controller.UpdateItemQuantity(id, newQuantity))
        {
            ShowMessage("Item updated successfully");
            RefreshView();
        }
        else
        {
            ShowMessage("Error updating item");
        }
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        if (_itemsGrid.SelectedRows.Count == 0)
        {
            ShowMessage("Please select an item to remove.");
            return;
        }

        string? selectedItemId = _itemsGrid.SelectedRows[0].Cells["Id"].Value?.ToString();
        if (string.IsNullOrEmpty(selectedItemId))
        {
            ShowMessage("Invalid item selection.");
            return;
        }

        if (_controller.RemoveItem(selectedItemId))
        {
            ShowMessage("Item deleted successfully");
            ClearInputs();
            RefreshView();
        }
        else
        {
            ShowMessage("Error deleting item");
        }
    }
}
