using DeployHelperWeb.DB;
using DeployHelperWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeployHelperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionItemsController : ControllerBase
    {
        private readonly VersionDbContext _context;

        public VersionItemsController(VersionDbContext context)
        {
            _context = context;
        }

        // GET: api/VersionItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VersionItem>>> GetVersionItems()
        {
            if (_context.VersionItems == null)
            {
                return NotFound();
            }
            return await _context.VersionItems.OrderByDescending(x => x.CreateTime).ToListAsync();
        }

        // GET: api/VersionItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VersionItem>> GetVersionItem(Guid id)
        {
            if (_context.VersionItems == null)
            {
                return NotFound();
            }
            var versionItem = await _context.VersionItems.FindAsync(id);

            if (versionItem == null)
            {
                return NotFound();
            }

            return versionItem;
        }

        // PUT: api/VersionItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVersionItem(Guid id, VersionItem versionItem)
        {
            if (id != versionItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(versionItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VersionItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/VersionItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VersionItem>> PostVersionItem(VersionItem versionItem)
        {
            if (_context.VersionItems == null)
            {
                return Problem("Entity set 'VersionDbContext.VersionItems'  is null.");
            }

            versionItem.CreateTime = DateTime.Now;
            versionItem.Creator = "Jono";
            _context.VersionItems.Add(versionItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVersionItem", new { id = versionItem.Id }, versionItem);
        }

        // DELETE: api/VersionItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVersionItem(Guid id)
        {
            if (_context.VersionItems == null)
            {
                return NotFound();
            }
            var versionItem = await _context.VersionItems.FindAsync(id);
            if (versionItem == null)
            {
                return NotFound();
            }

            _context.VersionItems.Remove(versionItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("last")]
        public async Task<ActionResult<VersionItem>> GetLastVersionItem()
        {
            if (_context.VersionItems?.Any() != true)
            {
                return NotFound();
            }

            return await _context.VersionItems.OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync();
        }

        private bool VersionItemExists(Guid id)
        {
            return (_context.VersionItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
