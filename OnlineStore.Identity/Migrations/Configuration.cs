namespace OnlineStore.Identity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineStore.Identity.IdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "OnlineStore.Identity.IdentityDbContext";
        }

        protected override void Seed(OnlineStore.Identity.IdentityDbContext context)
        {
        }
    }
}
