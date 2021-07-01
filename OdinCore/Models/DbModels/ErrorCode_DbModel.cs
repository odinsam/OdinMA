using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using OdinPlugs.OdinSqlSugar.SqlSugarExtends;
using OdinPlugs.OdinSqlSugar.SqlSugarInterface;
using SqlSugar;

namespace OdinCore.Models.DbModels
{

    [SugarTable("tb_errorcode")]
    public class ErrorCode_DbModel : IDbTable
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; } = OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();

        [OdinSugarColumn(Length = 256, IsNullable = true)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误码信息 - 对外描述
        /// </summary>
        /// <value></value>
        [OdinSugarColumn(Length = 512, IsNullable = true)]
        public string CodeShowMessage { get; set; }

        /// <summary>
        /// 错误码信息 - 对内描述
        /// </summary>
        /// <value></value>
        [OdinSugarColumn(Length = 512, IsNullable = true)]
        public string CodeErrorMessage { get; set; }

    }
}