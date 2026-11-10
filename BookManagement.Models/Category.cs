using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Range should be between 0 to 100!")]
        public int? DisplayOrder { get; set; }
    }
}
