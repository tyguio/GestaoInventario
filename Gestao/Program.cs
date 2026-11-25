using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        string dataFilePath = Path.Combine(AppContext.BaseDirectory, "inventory.json");
        var model = new InventoryModel(dataFilePath);
        var controller = new InventoryController(model);

        Application.Run(new InventoryFormView(controller));
    }
}
