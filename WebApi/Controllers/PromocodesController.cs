using DataLayer.Context;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocodesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromocodesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Promocodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocode>>> GetPromocodes()
        {
            return await _context.Promocodes.ToListAsync();
        }

        // GET: api/Promocodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Promocode>> GetPromocode(int id)
        {
            var promocode = await _context.Promocodes.FindAsync(id);

            if (promocode == null)
            {
                return NotFound();
            }

            return promocode;
        }

        // PUT: api/Promocodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromocode(int id, Promocode promocode)
        {
            if (id != promocode.Id)
            {
                return BadRequest();
            }

            _context.Entry(promocode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromocodeExists(id))
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

        // POST: api/Promocodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Promocode>> PostPromocode(Promocode promocode)
        {
            _context.Promocodes.Add(promocode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromocode", new { id = promocode.Id }, promocode);
        }

        // DELETE: api/Promocodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromocode(int id)
        {
            var promocode = await _context.Promocodes.FindAsync(id);
            if (promocode == null)
            {
                return NotFound();
            }

            _context.Promocodes.Remove(promocode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromocodeExists(int id)
        {
            return _context.Promocodes.Any(e => e.Id == id);
        }
    }
}
