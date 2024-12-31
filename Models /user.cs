using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        required public string Name { get; set; }

        [Required]
        [EmailAddress]
        required public string Email { get; set; }
    }
}
