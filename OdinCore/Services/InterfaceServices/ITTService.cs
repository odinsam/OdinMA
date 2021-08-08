using OdinCore.Models.OdinInterceptor;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinWebApi.OdinMvcCore.ServicesCore.ServicesInterface.SqlSugar;

namespace OdinCore.Services.InterfaceServices
{
    public interface ITTService : ISqlSugarServiceCore
    {
        OdinActionResult show();
    }
}