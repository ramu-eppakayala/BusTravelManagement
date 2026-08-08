using System.Data.Entity;
using BusTravelManagement.Models;

namespace BusTravelManagement.App_Start
{
    public class DatabaseConfig
    {
        public static void Initialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BusTravelDbContext>());
            using (var context = new BusTravelDbContext())
            {
                context.Database.Initialize(force: false);
            }
        }
    }
}
