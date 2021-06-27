using Microsoft.EntityFrameworkCore;

namespace OdinCore.Models.DbModels
{
    public class OdinProjectEntities : DbContext
    {
        public OdinProjectEntities(DbContextOptions<OdinProjectEntities> options) : base(options)
        {
        }

        /// <summary>
        /// 错误码
        /// </summary>
        /// <value></value>
        public DbSet<ErrorCode_DbModel> ErrorCodes { get; set; }

        public DbSet<Aop_ApiInvokerRecord_DbModel> ApiInvokerRecords { get; set; }

        public DbSet<Aop_ApiInvokerCatch_DbModel> ApiInvokerCatchs { get; set; }


    }
}