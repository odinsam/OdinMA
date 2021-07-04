using OdinCore.Models.DbModels;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinMAF.OdinEF.EFCore.EFExtensions;

namespace OdinCore.Services.ImplServices
{
    public class TTService : SqlSugarBaseRepository<ErrorCode_DbModel>, ITTService
    {
        public void show()
        {
            System.Console.WriteLine("this is tt service");
            throw new System.Exception("ttservice throw");
        }
    }
}