using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.EntityFramework;

namespace OnlineStore.Identity
{
    public static class UserRoles
    {
        public static List<IdentityUserRole> GetByUserID(string userID)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in db.Users
                            where item.Id == userID
                            select item.Roles.ToList();

                return query.Single();
            }
        }

        public static List<OSUser> GetByRoles(List<string> roleIDs)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in db.Users
                            where item.Roles.Any(role => roleIDs.Contains(role.RoleId))
                            select item;

                return query.ToList();
            }
        }

        public static string GetByRoleName(string roleName)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in db.Roles
                            where item.Name == roleName
                            select item.Id;

                return query.FirstOrDefault();
            }
        }
    }
}
