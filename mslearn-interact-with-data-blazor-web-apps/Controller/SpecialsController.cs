using BlazingPizza.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Controller;

[Route("specials")]
[ApiController]
public class SpecialsController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly PizzaStoreContext _db;

    public SpecialsController(PizzaStoreContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PizzaSpecial>>> GetSpecials()
    {
        var result = (await _db.Specials.ToListAsync()).OrderByDescending(s => s.BasePrice).ToList();
        return result;
    }


   
}
