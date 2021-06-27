using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OdinOIS.Models.DbModels.IdentityUserStore;

namespace OdinOIS.Models.DbModels
{
    public class OdinIdentityEntities : IdentityDbContext
    {
        public OdinIdentityEntities(DbContextOptions<OdinIdentityEntities> options) : base(options)
        {
        }
        public DbSet<IdUser> IdentityUsers { get; set; }
        public DbSet<IdUser> IdentityRoles { get; set; }
        public DbSet<IdentityUserClaim> IdentityUserClaim { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // mysql 修改索引长度 解决  Specified key was too long; max key length is 3072 bytes
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(256)");
                });
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("varchar(256)");
            });
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(256)");

                b.Property<string>("ProviderKey")
                    .HasColumnType("varchar(256)");
            });
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256)");
                });
            builder.Entity("OdinOIS.Models.DbModels.IdentityUserStore.IdentityUserClaim", b =>
            {
                b.Property<string>("ClaimId")
                    .HasColumnType("varchar(256)");

            });
        }
    }
}