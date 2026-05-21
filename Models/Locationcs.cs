using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop111.Models
{
    [Table("Location")]
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Area { get; set; }

        // এক লোকেশনে অনেক ভেন্ডর থাকতে পারে
        public virtual ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    }
}
