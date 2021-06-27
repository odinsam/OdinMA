using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using Odin.Plugs.OdinSqlSugar.SqlSugarExtends;
using Odin.Plugs.OdinSqlSugar.SqlSugarInterface;
using SqlSugar;

namespace OdinCore.Models.DbModels
{
    [SugarTable("tb_aop_apiinvokerexception")]
    public class Aop_ApiInvokerException_DbModel : IDbTable
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        /// <returns></returns>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();

        [OdinSugarStringColumn(Length = 128)]
        public string Guid { get; set; }

        [OdinSugarStringColumn(Length = 64)]
        public string ControllerName { get; set; }

        [OdinSugarStringColumn(Length = 64)]
        public string ActionName { get; set; }

        [OdinSugarStringColumn(Length = 128)]
        public string ErrorMessage { get; set; }

        [OdinSugarStringColumn]
        public string ErrorInfo { get; set; }


        public long ErrorTime { get; set; }

    }
}