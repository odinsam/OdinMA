using OdinCore.Models.OdinInterceptor;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinMvcCore.ServicesCore.ServicesInterface.SqlSugar;

namespace OdinCore.Services.InterfaceServices
{
    public interface ITTService : ISqlSugarServiceCore
    {
        OdinActionResult show();
    }
}