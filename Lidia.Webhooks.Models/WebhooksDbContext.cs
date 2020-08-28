using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models
{
    public class WebhooksDbContext : DbContext
    {
        public WebhooksDbContext() : base("Lidia.Webhooks")
        {
            // Disable initializer and deactivate data model comparizon against the db
            Database.SetInitializer<WebhooksDbContext>(null);
        }

        public static WebhooksDbContext Create()
        {
            return new WebhooksDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region [ Schema ]

            modelBuilder.HasDefaultSchema("lw");

            #endregion
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<Webhook> Webhooks { get; set; }
    }
}
