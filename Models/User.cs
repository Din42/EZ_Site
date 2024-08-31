using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EZ_Site.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Password { get; set; }
        
        public string Role { get; set; }
    }
}
