using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using Odin.Plugs.OdinSqlSugar.SqlSugarExtends;
using Odin.Plugs.OdinSqlSugar.SqlSugarInterface;
using SqlSugar;

namespace OdinCore.Models.DbModels
{
    [SugarTable("tb_aop_apiinvokercatch")]
    public class Aop_ApiInvokerCatch_DbModel : IDbTable
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        /// <returns></returns>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();

        [OdinSugarStringColumn(Length = 128)]
        public string Guid { get; set; }
        /// <summary>
        /// ControllerName
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 64)]
        public string ControllerName { get; set; }
        /// <summary>
        /// ActionName
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 64)]
        public string ActionName { get; set; }

        [OdinSugarStringColumn(Length = 256)]
        public string ApiPath { get; set; }

        [OdinSugarStringColumn]
        public string StrParam { get; set; }
        /// <summary>
        /// ControllerName
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 512)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// error ShowMessage
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 512)]
        public string ShowMessage { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 128)]
        public string ErrorCode { get; set; }

        [OdinSugarStringColumn(IsNullable = true)]
        public long ErrorTime { get; set; }

        /// <summary>
        /// Author Name
        /// </summary>
        /// <returns></returns>
        [OdinSugarStringColumn(Length = 128)]
        public string Author { get; set; }


        [OdinSugarStringColumn]
        public string ExceptionMessage { get; set; }
    }
}