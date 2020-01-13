using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.Linq;
using OnlineStore.EntityFramework;

namespace OnlineStore.Identity
{
    public class IdentityDbContext : IdentityDbContext<OSUser>
    {
        public IdentityDbContext()
            : base(Global.ConnectionString)
        {

        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OSUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
        }

        public static IdentityDbContext Entity
        {
            get
            {
                return new IdentityDbContext();
            }
        }
    }
}