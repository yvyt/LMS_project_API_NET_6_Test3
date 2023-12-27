using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Data
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [MaxLength(255)]
        [Required]
        public string Password { get; set; }

        [MaxLength(255)]
        [Required]

        public string FirstName { get; set; }

        [MaxLength(255)]
        [Required]

        public string LastName { get; set; }

        [MaxLength(10)]
        [Required]

        public string Gender { get; set; }

        [MaxLength(20)]
        [Required]
        public string Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(255)]

        public string Avatar { get; set; } = "default.png";

        public bool IsFirstLogin { get; set; } = true;

        [MaxLength(255)]
        [Required]
        public string Address { get; set; }
        [Required]
        public string RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Role Roles { get; set; }
        public virtual ICollection<UserPermission> Permissions { get; set; }
    }
}
