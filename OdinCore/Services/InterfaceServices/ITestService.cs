using OdinPlugs.OdinModels.ErrorCode;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinWebApi.OdinMvcCore.ServicesCore.ServicesInterface.SqlSugar;

namespace OdinCore.Services.InterfaceServices
{
    // [ServiceInterceptor(typeof(FoobarAttribute))]
    public interface ITestService : ISqlSugarServiceCore
    {
        OdinActionResult show(long id);
        OdinActionResult showPost();
        OdinActionResult InsertErrorCode(ErrorCode_Model errorCode);
        OdinActionResult UpdateErrorCode(ErrorCode_Model errorCode, long id);
        OdinActionResult DeleteErrorCode(long id);
        OdinActionResult SelectErrorCode(long id);
        OdinActionResult SelectErrorCodeByCache(string key);
    }
}