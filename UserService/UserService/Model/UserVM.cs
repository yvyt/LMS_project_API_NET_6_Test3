using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UserService.Data;

namespace UserService.Model
{
    public class UserVM
    {
        public string? Id { get; set; }
        public string Email { get; set; }

        
        public string Password { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

       
        public string Gender { get; set; }

      
        public string Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }


        public string Avatar { get; set; }

        public bool IsFirstLogin { get; set; } = true;

     
        public string Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string RoleId {  get; set; }
       
    }
}
