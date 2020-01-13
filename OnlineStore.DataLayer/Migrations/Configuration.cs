namespace OnlineStore.DataLayer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineStore.DataLayer.OnlineStoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "OnlineStore.DataLayer.OnlineStoreDbContext";
        }

        protected override void Seed(OnlineStore.DataLayer.OnlineStoreDbContext context)
        {
            
        }
    }
}
