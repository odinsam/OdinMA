using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdinIds.Models.DbModels.IdentityUserStore
{
    public class IdentityUserClaim
    {
        [Key]
        public string ClaimId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string UserSubjectId { get; set; }
        [ForeignKey("UserSubjectId")]
        public virtual IdUser IdentityUser { get; set; }
    }
}