using System.Data.Common;
using System.Collections.Generic;

namespace OdinOIS.Models.DbModels
{
    public class SampleData
    {
        public SampleData()
        {

        }
        public static void Init(OdinIdentityEntities context)
        {

            context.SaveChanges();
        }
    }
}