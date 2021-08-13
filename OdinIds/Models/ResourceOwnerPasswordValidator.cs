using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace OdinIds.Models
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.UserName == "odinsam" && context.Password == "123")
            {
                context.Result = new GrantValidationResult(
                    subject: "odinsam",
                    authenticationMethod: "custom",

                    claims: new Claim[] { new Claim("name", context.UserName) }
                );
            }
            else
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "custom client error");
            return Task.CompletedTask;
        }
    }
}