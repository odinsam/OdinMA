using System.Collections.Generic;
using IdentityServer4.Models;
using OdinOIS.Models.DbModels.IdentityUserStore;

namespace OdinOIS.Models
{
    public static class IdentityConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }
        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };

        // public static IEnumerable<ApiResource> GetApis()
        // {
        //     return new List<ApiResource>
        //     {
        //         new ApiResource("api1", "My API")
        //     };
        // }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
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
                    SubjectId = "1",
                    UserName = "alice",
                    Password = "password"
                },
                new IdUser
                {
                    SubjectId = "2",
                    UserName = "bob",
                    Password = "password"
                }
            };
        }
    }
}