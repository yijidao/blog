using BlazingPizza.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Controller;

[Route("orders")]
[ApiController]
public class OrdersController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly PizzaStoreContext _db;

    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        var orders = await _db.Orders
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings)
            .OrderByDescending(o => o.CreatedTime)
            .ToListAsync();
        return orders.Select(OrderWithStatus.FromOrder).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        order.CreatedTime = DateTime.Now;

        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id;
            pizza.Special = null;
        }

        _db.Orders.Attach(order);
        await _db.SaveChangesAsync();
        return order.OrderId;

    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
    {
        var order = await _db.Orders
            .Where(x => x.OrderId == orderId)
            .Include(x => x.Pizzas).ThenInclude(x => x.Special)
            .Include(x => x.Pizzas).ThenInclude(x => x.Toppings).ThenInclude(x => x.Topping)
            .SingleOrDefaultAsync();

        return order == null ? NotFound() : OrderWithStatus.FromOrder(order);
    }
}