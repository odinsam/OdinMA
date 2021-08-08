using Microsoft.AspNetCore.Mvc;
using OdinPlugs.OdinInject.InjectInterface;
using OdinPlugs.OdinWebApi.OdinCore.Models;

namespace OdinCore.Services.InterfaceServices
{
    public interface IInerService : IAutoInject
    {
        OdinActionResult show(long id);
    }
}