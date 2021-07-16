using System.Data.Common;
using System.Collections.Generic;
using SqlSugar.IOC;
using Newtonsoft.Json;
using OdinPlugs.OdinInject;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
using OdinPlugs.OdinInject.InjectCore;

namespace OdinCore.Models.DbModels
{
    public class SampleData
    {
        public SampleData()
        {

        }
        public static void Init()
        {
            var odinSnowFlake = OdinInjectCore.GetService<IOdinSnowFlake>();
            List<ErrorCode_DbModel> errorCodes = new List<ErrorCode_DbModel>
                {
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(), ErrorCode = "ok", CodeErrorMessage = "", CodeShowMessage = "" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-error", CodeErrorMessage = "系统异常，请联系管理员", CodeShowMessage = "系统异常:[sys-error]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-error-undefind", CodeErrorMessage = "错误码不存在", CodeShowMessage = "错误码不存在:[sys-error-undefind]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-requestparams-null", CodeErrorMessage = "请求参数不存在", CodeShowMessage = "请求参数不存在:[sys-requestparams-null]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-requestparams-undefind", CodeErrorMessage = "缺少请求参数不存在", CodeShowMessage = "缺少请求参数不存在:[sys-requestparams-undefind]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-apilink", CodeErrorMessage = "请求缺少Guid", CodeShowMessage = "请求缺少Guid:[sys-apilink]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-timeout", CodeErrorMessage = "请求超时", CodeShowMessage = "请求超时:[sys-timeout]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-paramsign-undefind", CodeErrorMessage = "请求缺少签名[sign]", CodeShowMessage = "请求缺少签名-sign:[sys-paramsign-undefind]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-paramsign-error", CodeErrorMessage = "请求验签失败", CodeShowMessage = "请求验签失败:[sys-paramsign-error]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-allowip", CodeErrorMessage = "ip不在白名单，无法请求接口", CodeShowMessage = "ip不在白名单，无法请求接口:[sys-allowip]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-allowip-disallowips", CodeErrorMessage = "ip在黑名单，无法请求接口", CodeShowMessage = "ip在黑名单，无法请求接口:[sys-allowip-disallowips]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-param-undefind", CodeErrorMessage = "方法调用缺少参数", CodeShowMessage = "方法调用缺少参数:[sys-param-undefind]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-token-undefind", CodeErrorMessage = "请求header缺少[Token]", CodeShowMessage = "请求header缺少-token:[sys-token-undefind]" },
                    new ErrorCode_DbModel {Id = odinSnowFlake.CreateSnowFlakeId(),ErrorCode = "sys-catcherror", CodeErrorMessage = "系统运行catch异常", CodeShowMessage = "系统运行catch异常，错误以记录会尽快处理:[sys-catcherror]" },

                };
            DbScoped.Sugar.Insertable<ErrorCode_DbModel>(errorCodes).ExecuteCommand();
        }
        // public static void Init(OdinProjectEntities context)
        // {
        //     List<ErrorCode_DbModel> errorCodes = new List<ErrorCode_DbModel>
        //     {
        //         new ErrorCode_DbModel { Id=1, ErrorCode = "ok", CodeErrorMessage = "", CodeShowMessage = "" },
        //         new ErrorCode_DbModel { Id=2, ErrorCode = "sys-error", CodeErrorMessage = "系统异常，请联系管理员", CodeShowMessage = "系统异常" },
        //         new ErrorCode_DbModel { Id=3, ErrorCode = "sys-requestparams-null", CodeErrorMessage = "请求参数不存在", CodeShowMessage = "请求参数不存在" },
        //         new ErrorCode_DbModel { Id=4, ErrorCode = "sys-requestparams-undefind", CodeErrorMessage = "缺少请求参数不存在", CodeShowMessage = "缺少请求参数不存在" },
        //         new ErrorCode_DbModel { Id=5, ErrorCode = "sys-apilink", CodeErrorMessage = "请求缺少Guid", CodeShowMessage = "请求缺少Guid" },
        //         new ErrorCode_DbModel { Id=6, ErrorCode = "sys-timeout", CodeErrorMessage = "请求超时", CodeShowMessage = "请求超时" },
        //         new ErrorCode_DbModel { Id=7, ErrorCode = "sys-paramsign-undefind", CodeErrorMessage = "请求缺少签名[sign]", CodeShowMessage = "请求缺少签名[sign]" },
        //         new ErrorCode_DbModel { Id=8, ErrorCode = "sys-paramsign-error", CodeErrorMessage = "请求验签失败", CodeShowMessage = "请求验签失败" },
        //         new ErrorCode_DbModel { Id=9, ErrorCode = "sys-allowip", CodeErrorMessage = "ip不在白名单，无法请求接口", CodeShowMessage = "ip不在白名单，无法请求接口" },
        //         new ErrorCode_DbModel { Id=10, ErrorCode = "sys-allowip-disallowips", CodeErrorMessage = "ip在黑名单，无法请求接口", CodeShowMessage = "ip在黑名单，无法请求接口" },
        //         new ErrorCode_DbModel { Id=11, ErrorCode = "sys-param-undefind", CodeErrorMessage = "方法调用缺少参数", CodeShowMessage = "方法调用缺少参数" },
        //         new ErrorCode_DbModel { Id=12, ErrorCode = "sys-token-undefind", CodeErrorMessage = "请求header缺少[Token]", CodeShowMessage = "请求header缺少[Token]" },
        //         new ErrorCode_DbModel { Id=13, ErrorCode = "sys-catcherror", CodeErrorMessage = "系统运行catch异常", CodeShowMessage = "系统运行catch异常，错误以记录会尽快处理" },
        //     };
        //     context.ErrorCodes.AddRange(errorCodes);

        //     context.SaveChanges();
        // }
    }
}