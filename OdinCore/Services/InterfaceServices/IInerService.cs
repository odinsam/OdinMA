using Microsoft.AspNetCore.Mvc;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinInject.InjectInterface;

namespace OdinCore.Services.InterfaceServices
{
    public interface IInerService : IAutoInject
    {
        OdinActionResult show(long id);
    }
}