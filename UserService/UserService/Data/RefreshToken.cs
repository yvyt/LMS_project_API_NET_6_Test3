using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Data
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string id {  get; set; }
        
        public string UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public User owner { get; set; }
        public string Token {  get; set; }
        public string jwt {  get; set; }
        public bool isUse {  get; set; }
        public bool isRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set;}

    }
}
