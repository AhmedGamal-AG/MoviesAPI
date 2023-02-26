using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _context.Genres.OrderBy(g=>g.Name).ToListAsync();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromBody] CreateGenreDto genredto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g=>g.Id==id);
            if(genre==null)
            { return NotFound($"No Genre Was Found With{id}"); }
            genre.Name = genredto.Name;
            await _context.SaveChangesAsync();
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            { return NotFound($"No Genre Was Found With{id}"); }
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return Ok(genre);
        }
    }
}
