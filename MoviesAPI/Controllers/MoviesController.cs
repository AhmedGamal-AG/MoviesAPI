using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private List<string> allowedExtensions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(ApplicationDbContext _context)
        {
            this._context = _context;
        }



        [HttpGet]

        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).Include(m => m.Genre).
                Select(m => new
                {
                    MovieId = m.Id,
                    MovieRate = m.Rate,
                    MovieYear = m.Year,
                    MovieGenreName = m.Genre.Name
                }
                      ).ToListAsync();
            return Ok(movies);
        }

        [HttpGet("{Id}")]

        public async Task<IActionResult> GetMovieByIDAsync(int Id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(x => x.Id == Id);

            if (movie == null)
            {
                return NotFound($"No Movie With ID={Id}");
            }

            var movieDetails = new
            {
                MovieId = movie.Id,
                MovieRate = movie.Rate,
                MovieYear = movie.Year,
                MovieGenreName = movie.Genre.Name
            };

            return Ok(movieDetails);

        }


        [HttpGet("GetMoviesByGenreID")]

        public async Task<IActionResult> GetMoviesByGenreIDAsync(Byte genreId)
        {
            var movies = await _context.Movies.Where(m => m.GenreId == genreId).OrderByDescending(m => m.Rate).Include(m => m.Genre).
                Select(m => new
                {
                    MovieId = m.Id,
                    MovieRate = m.Rate,
                    MovieYear = m.Year,
                    MovieGenreName = m.Genre.Name
                }
                      ).ToListAsync();


            return Ok(movies);

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (!allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only Png and Jpg is allowed");
            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("MaxSize is 1 MB");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenre)
            {
                return BadRequest("l Genre Id");
            }


            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            Movie movie = new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Poster = dataStream.ToArray(),
                GenreId = dto.GenreId
            };

            await _context.AddAsync(movie);
            await _context.SaveChangesAsync();

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(g => g.Id == id);
            if (movie == null)
            { return NotFound($"No Movie Was Found With{id}"); }
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult>UpdateAsync(int Id  ,[FromForm]MovieDto dto)
        {
        var movie = await _context.Movies.SingleOrDefaultAsync(g => g.Id == Id);
        if (movie == null)
        { return NotFound($"No Movie Was Found With{Id}"); }


            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenre)
            {
                return BadRequest("Genre Id is Not Found");
            }
            if (dto.Poster!=null)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only Png and Jpg is allowed");
                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("MaxSize is 1 MB");
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }


            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.GenreId = dto.GenreId;
 
            await _context.SaveChangesAsync();

            return Ok(movie);
        }
    }
}
