using Microsoft.AspNetCore.Mvc;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinMvcCore.OdinInject.InjectInterface;

namespace OdinCore.Services.InterfaceServices
{
    public interface IInerService : IAutoInject
    {
        OdinActionResult show(long id);
    }
}