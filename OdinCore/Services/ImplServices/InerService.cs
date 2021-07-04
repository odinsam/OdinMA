using System;
using Microsoft.AspNetCore.Mvc;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinCore.Models;

namespace OdinCore.Services.ImplServices
{
    public class InerService : IInerService
    {
        public OdinActionResult show(long id)
        {
            System.Console.WriteLine($"this is InerService show method:{ id }");
            throw new Exception("ITService throw exception");
            return new OdinActionResult
            {
                Data = $"this is TestService show method:{id}"
            };
        }
    }
}