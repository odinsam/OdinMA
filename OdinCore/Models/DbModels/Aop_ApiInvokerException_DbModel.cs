using OdinPlugs.OdinInject;
using OdinPlugs.OdinSqlSugar.SqlSugarExtends;
using OdinPlugs.OdinSqlSugar.SqlSugarInterface;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
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
        public long Id { get; set; } = OdinInjectCore.GetService<IOdinSnowFlake>().CreateSnowFlakeId();

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