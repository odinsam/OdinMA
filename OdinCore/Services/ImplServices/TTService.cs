using System;
using System.Xml;
using OdinCore.Models.DbModels;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinEFCore.EntityFrameworkExtensions.EFExtensions;
using OdinPlugs.OdinWebApi.OdinCore.Models;

namespace OdinCore.Services.ImplServices
{
    public class TTService : SqlSugarBaseRepository<ErrorCode_DbModel>, ITTService
    {
        public OdinActionResult show()
        {
            System.Console.WriteLine("this is tt service");

            throw new System.Exception("ttservice throw");
            return this.OdinResult("this is TTService method");
        }

    }
}