using System.ComponentModel.DataAnnotations;

namespace UserService.Model
{
    public class UserLogin
    {
        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [MaxLength(255)]
        [Required]
        public string Password { get; set; }

    }

}
