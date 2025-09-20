using DataLayer.Context;
using DataLayer.DTOs;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/promocodes")]
    [ApiController]
    public class PromocodesController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocode>>> GetPromocodes()
        {
            var promocodes = await context.Promocodes.ToListAsync();

            if (!promocodes.Any())
                return NotFound(new Response("Promocodes not found", StatusCodes.Status404NotFound));
            return promocodes;
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Promocode>> GetPromocode(string code)
        {
            var promocode = await context.Promocodes.FindAsync(code);

            if (promocode == null)
                return NotFound(new Response("Promocode not found", StatusCodes.Status404NotFound));

            return promocode;
        }


        [HttpPut("{code}/activate")]
        public async Task<IActionResult> ActivatePromocode(string code)
        {
            var promocode = await context.Promocodes
                .FirstOrDefaultAsync(p => p.Code == code);

            if (promocode == null)
                return NotFound(new Response("Promocode not found", StatusCodes.Status404NotFound));
            if (!promocode.IsActive)
                return Ok(new Response("The promocodeDto has already been used (deactivated).", StatusCodes.Status200OK));
            if (promocode.StartDate.HasValue && DateTime.UtcNow < promocode.StartDate.Value)
                return BadRequest(new Response($"Promocode is not active yet. It will be available from {promocode.StartDate.Value:yyyy-MM-dd HH:mm:ss} UTC.",StatusCodes.Status400BadRequest));
            if (promocode.EndDate.HasValue && DateTime.UtcNow > promocode.EndDate.Value)
                return BadRequest(new Response($"Promocode has expired. It was valid until {promocode.EndDate.Value:yyyy-MM-dd HH:mm:ss} UTC.", StatusCodes.Status400BadRequest));

            promocode.IsActive = false;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500 ,new Response("Internal server error. Please try again later.", 500));
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Promocode>> PostPromocode(PostPromocodeDto promocodeDto)
        {
            if (string.IsNullOrWhiteSpace(promocodeDto.Code))
                return BadRequest(new Response("The promocode cannot be empty.", StatusCodes.Status400BadRequest));

            var promocode = new Promocode
            {
                Code = promocodeDto.Code,
                StartDate = promocodeDto.StartDate,
                EndDate = promocodeDto.EndDate,
            };

            context.Promocodes.Add(promocode);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                    return Conflict(new Response("A promocode with this code already exists.",StatusCodes.Status409Conflict));
                return BadRequest(new Response("Invalid data provided. Please check your input.", StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response("Internal server error. Please try again later.", StatusCodes.Status500InternalServerError));
            }

            return CreatedAtAction("GetPromocode", new { code = promocodeDto.Code }, promocodeDto);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeletePromocode(string code)
        {
            var promocode = await context.Promocodes.FirstOrDefaultAsync(p => p.Code == code);
            if (promocode == null)
                return NotFound(new Response("Promocode not found", StatusCodes.Status404NotFound));

            context.Promocodes.Remove(promocode);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new Response("Internal server error. Please try again later.", StatusCodes.Status500InternalServerError));
            }

            return NoContent();
        }
    }
}
