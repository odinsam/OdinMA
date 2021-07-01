using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using OdinPlugs.OdinSqlSugar.SqlSugarExtends;
using OdinPlugs.OdinSqlSugar.SqlSugarInterface;
using SqlSugar;

namespace OdinCore.Models.DbModels
{
    [SugarTable("tb_aop_apiinvokerrecord")]
    public class Aop_ApiInvokerRecord_DbModel : IDbTable
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        /// <returns></returns>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();

        [OdinSugarStringColumn(Length = 64, IsNullable = true)]
        public string GUID { get; set; } = "";

        /// <summary>
        /// ControllerName
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 512, IsNullable = true)]
        public string ControllerName { get; set; }

        /// <summary>
        /// ActionName
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 512, IsNullable = true)]
        public string ActionName { get; set; }

        /// <summary>
        /// 入参
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 4096, IsNullable = true)]
        public string InputParams { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 8192, IsNullable = true)]
        public string ReturnValue { get; set; }

        /// <summary>
        /// 调用方式
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 8, IsNullable = true)]
        public string CallMethod { get; set; }

        [OdinSugarColumn]
        public long? BeginTime { get; set; }

        [OdinSugarColumn]
        public long? EndTime { get; set; }

        [OdinSugarColumn]
        public long? CallTimeSpan { get; set; }
        [OdinSugarStringColumn(Length = 128, IsNullable = true)]
        public string LogicState { get; set; } = "ok";

        [OdinSugarColumn]
        public int? StateCode { get; set; }
    }
}