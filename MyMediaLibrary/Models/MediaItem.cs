using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMediaLibrary.Models
{
    public class MediaItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Author { get; set; } = "Неизвестный автор";

        [MaxLength(50)]
        public string Genre { get; set; } = "Книга";

        public string Description { get; set; }
        public bool IsVisited { get; set; }

        [Required]
        [Column(TypeName = "REAL")]
        public double Rating { get; set; } = 0.0;

        [MaxLength(500)]
        public string ImagePath { get; set; }
    }
}