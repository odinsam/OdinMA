using System;
using Microsoft.AspNetCore.Mvc;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinMvcCore.OdinExtensions;

namespace OdinCore.Services.ImplServices
{
    public class InerService : IInerService
    {
        private readonly ITTService ttService;

        public InerService()
        {
            this.ttService = MvcContext.GetRequiredServices<ITTService>();
        }
        public OdinActionResult show(long id)
        {
            System.Console.WriteLine($"this is InerService show method:{ id }");
            return this.ttService.show();
            // throw new Exception("ITService throw exception");
            // return new OdinActionResult
            // {
            //     Data = $"this is TestService show method:{id}"
            // };
        }
    }
}