using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.EntityFramework;
using System;

namespace OnlineStore.Identity
{
    public static class Roles
    {
        private static IQueryable<IdentityRole> _cachedRoles
        {
            get
            {
                using (var db = IdentityDbContext.Entity)
                    return db.Roles.ToCacheableList().AsQueryable();
            }
        }

        public static List<IdentityRole> Get()
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in _cachedRoles
                            select item;

                return query.ToList();
            }
        }

        public static IdentityRole GetByID(string roleID)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in _cachedRoles
                            where item.Id == roleID
                            select item;

                return query.Single();
            }
        }
    }
}
