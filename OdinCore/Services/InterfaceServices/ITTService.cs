using OdinCore.Models.OdinInterceptor;
using OdinPlugs.OdinMvcCore.OdinInject.InjectInterface;
using OdinPlugs.OdinMvcCore.ServicesCore.ServicesInterface.SqlSugar;

namespace OdinCore.Services.InterfaceServices
{
    public interface ITTService : ISqlSugarServiceCore
    {
        void show();
    }
}