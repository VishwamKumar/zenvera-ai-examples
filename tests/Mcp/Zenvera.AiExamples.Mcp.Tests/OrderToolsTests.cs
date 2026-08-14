namespace Zenvera.AiExamples.Mcp.Tests;

public sealed class OrderToolsTests
{
    [Fact]
    public void GetOrderDetails_returns_known_order()
    {
        var tools = new OrderTools();
        tools.GetOrderDetails("ORD-1001").Should().Contain("Asha Patel");
    }

    [Fact]
    public void GetOrderDetails_returns_not_found()
    {
        var tools = new OrderTools();
        tools.GetOrderDetails("ORD-9999").Should().Contain("not found");
    }
}
