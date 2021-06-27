using Microsoft.EntityFrameworkCore;

namespace OdinServerManagerApi.Models.DbModels
{
    public class OdinIdentityEntities : DbContext
    {
        public OdinIdentityEntities(DbContextOptions<OdinIdentityEntities> options) : base(options)
        {
        }

    }
}