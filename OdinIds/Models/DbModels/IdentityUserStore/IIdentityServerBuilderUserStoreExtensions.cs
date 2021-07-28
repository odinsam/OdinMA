using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OdinIds.Models.DbModels.IdentityUserStore
{
    public static class IIdentityServerBuilderUserStoreExtensions
    {
        public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder builder, Action<DbContextOptionsBuilder> userStoreOptions = null)
        {
            builder.Services.AddDbContext<OdinIdentityEntities>(userStoreOptions);
            builder.Services.AddTransient<UserStore>();
            return builder;
        }
    }
}