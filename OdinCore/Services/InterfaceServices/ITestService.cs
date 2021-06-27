using Odin.Plugs.OdinCore.Models;
using Odin.Plugs.OdinCore.Models.ErrorCode;
using Odin.Plugs.OdinMvcCore.ServicesCore.ServicesInterface.SqlSugar;

namespace OdinCore.Services.InterfaceServices
{
    public interface ITestService : ISqlSugarServiceCore
    {
        OdinActionResult show();
        OdinActionResult showPost();
        OdinActionResult InsertErrorCode(ErrorCode_Model errorCode);
        OdinActionResult UpdateErrorCode(ErrorCode_Model errorCode, long id);
        OdinActionResult DeleteErrorCode(long id);
        OdinActionResult SelectErrorCode(long id);
        OdinActionResult SelectErrorCodeByCache(string key);
    }
}