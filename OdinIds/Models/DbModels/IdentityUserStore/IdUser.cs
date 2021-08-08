using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OdinIds.Models.DbModels.IdentityUserStore
{
    public class IdUser : IdentityUser
    {
        [Required]
        public string SubjectId { get; set; }
        [Required]
        public string Password { get; set; }
        public string ProviderName { get; set; }
        public string ProviderSubjectId { get; set; }
        public bool IsActive { get; set; }
        // public ICollection<IdentityUserClaim> IdentityUserClaims { get; set; }

    }
}