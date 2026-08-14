namespace Zenvera.AiExamples.Mcp.OrdersStdioHost.Tools;

internal sealed class OrderTools
{
    private static readonly Dictionary<string, object> Orders = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ORD-1001"] = new
        {
            Customer = "Asha Patel",
            Total = "$150.00",
            Status = "Shipped",
            Items = new[] { "Ridgewalker Trail Boot", "Merino Socks" },
            ShippingAddress = "12 Summit Lane, Trail City",
            OrderDate = "2026-07-25",
            TrackingNumber = "ZV1001TRACK"
        },
        ["ORD-1002"] = new
        {
            Customer = "Jordan Lee",
            Total = "$89.99",
            Status = "Processing",
            Items = new[] { "Trail Running Shoes" },
            ShippingAddress = "44 Ridge Road, Peak Town",
            OrderDate = "2026-07-30",
            TrackingNumber = (string?)null
        },
        ["ORD-1003"] = new
        {
            Customer = "Sam Rivera",
            Total = "$245.50",
            Status = "Delivered",
            Items = new[] { "Daypack", "Insulated Bottle", "Head Torch" },
            ShippingAddress = "9 Valley Way, River Bend",
            OrderDate = "2026-07-20",
            TrackingNumber = "ZV1003TRACK"
        }
    };

    [McpServerTool]
    [Description("Retrieves order information from the Zenvera Outfitters catalog.")]
    public string GetOrderDetails(
        [Description("The order ID to look up, for example ORD-1001")] string orderId)
    {
        if (Orders.TryGetValue(orderId, out var order))
        {
            return JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true });
        }

        return $"Order {orderId} was not found.";
    }

    [McpServerTool]
    [Description("Searches for orders by customer name.")]
    public string SearchOrdersByCustomer(
        [Description("Customer name to search for")] string customerName)
    {
        var matches = Orders
            .Where(pair => pair.Value.ToString()?.Contains(customerName, StringComparison.OrdinalIgnoreCase) == true
                || GetCustomer(pair.Key)?.Contains(customerName, StringComparison.OrdinalIgnoreCase) == true)
            .Select(pair => new { OrderId = pair.Key, pair.Value })
            .ToArray();

        if (matches.Length == 0)
        {
            return $"No orders found for '{customerName}'.";
        }

        return JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool]
    [Description("Returns inventory availability for a product name.")]
    public string GetInventory(
        [Description("Product name, for example Ridgewalker Trail Boot")] string productName)
    {
        var inventory = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["Ridgewalker Trail Boot"] = new { InStock = 14, Warehouse = "East" },
            ["Trail Running Shoes"] = new { InStock = 3, Warehouse = "West" },
            ["Daypack"] = new { InStock = 22, Warehouse = "East" }
        };

        if (inventory.TryGetValue(productName, out var stock))
        {
            return JsonSerializer.Serialize(stock, new JsonSerializerOptions { WriteIndented = true });
        }

        return $"No inventory record for '{productName}'.";
    }

    private static string? GetCustomer(string orderId) => orderId switch
    {
        "ORD-1001" => "Asha Patel",
        "ORD-1002" => "Jordan Lee",
        "ORD-1003" => "Sam Rivera",
        _ => null
    };
}
