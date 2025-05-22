using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMediaLibrary.Models
{
    public enum MediaType
    {
        Book,
        Movie,
        Music
    }

    public class MediaItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public bool IsVisited { get; set; }
        public string ImagePath { get; set; }
        public MediaType Type { get; set; }
    }
}
