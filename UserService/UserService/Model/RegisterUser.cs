using System.ComponentModel.DataAnnotations;

namespace UserService.Model
{
    public class RegisterUser
    {
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
        [Required]
        public string Role {  get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }


    }
}
