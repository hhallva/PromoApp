using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Models;

namespace WebApi.Controllers
{
    [Route("api/promocodes")]
    [ApiController]
    public class PromocodesController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocode>>> GetPromocodes()
        {
            return await context.Promocodes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promocode>> GetPromocode(string id)
        {
            var promocode = await context.Promocodes.FindAsync(id);

            if (promocode == null)
            {
                return NotFound();
            }

            return promocode;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromocode(string id, Promocode promocode)
        {
            if (id != promocode.Code)
            {
                return BadRequest();
            }

            context.Entry(promocode).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
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

        [HttpPost]
        public async Task<ActionResult<Promocode>> PostPromocode(Promocode promocode)
        {
            context.Promocodes.Add(promocode);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PromocodeExists(promocode.Code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPromocode", new { id = promocode.Code }, promocode);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromocode(string id)
        {
            var promocode = await context.Promocodes.FindAsync(id);
            if (promocode == null)
            {
                return NotFound();
            }

            context.Promocodes.Remove(promocode);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromocodeExists(string id)
        {
            return context.Promocodes.Any(e => e.Code == id);
        }
    }
}
