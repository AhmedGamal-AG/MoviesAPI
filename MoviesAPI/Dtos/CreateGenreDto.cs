using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Dtos
{
    public class CreateGenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
