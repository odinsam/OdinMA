using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using OdinIds.Models.DbModels;
using OdinIds.Models.DbModels.IdentityUserStore;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;

namespace OdinIds.Models
{
    public static class IdentityConfig
    {
        public static IEnumerable<IdentityServer4.Models.IdentityResource> GetIdentityResources()
        {
            return new IdentityServer4.Models.IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }
        public static IEnumerable<IdentityServer4.Models.ApiScope> ApiScopes =>
        new List<IdentityServer4.Models.ApiScope>
        {
            new IdentityServer4.Models.ApiScope("api1", "My API")
        };

        // public static IEnumerable<ApiResource> GetApis()
        // {
        //     return new List<ApiResource>
        //     {
        //         new ApiResource("api1", "My API")
        //     };
        // }

        public static IEnumerable<IdentityServer4.Models.Client> GetClients()
        {
            return new List<IdentityServer4.Models.Client>
            {
                new IdentityServer4.Models.Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new IdentityServer4.Models.Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                }
            };
        }

        public static List<IdUser> GetUsers()
        {
            return new List<IdUser>
            {
                new IdUser
                {
                    Id = OdinInjectCore.GetService<IOdinSnowFlake>().CreateSnowFlakeId().ToString(),
                    SubjectId = "1",
                    UserName = "alice",
                    Password = "password"
                },
                new IdUser
                {
                    Id = OdinInjectCore.GetService<IOdinSnowFlake>().CreateSnowFlakeId().ToString(),
                    SubjectId = "2",
                    UserName = "bob",
                    Password = "password"
                }
            };
        }

        public static List<Student_DbModel> GetStus()
        {
            return new List<Student_DbModel>
            {
                new Student_DbModel
                {
                    Id = 1,
                    StudentName = "odinsam",
                }
            };
        }
    }
}