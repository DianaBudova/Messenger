using Messenger.DAL;
using Messenger.Models.DB;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Messenger.Startup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using DataContext context = new();
            if (!(context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)!.Exists())
                context.Database.EnsureCreated();

            this.ConfigureTables(context);
        }

        private void ConfigureTables(DataContext context)
        {
            if (context.Server.Any())
                return;
            string? serverNameByDefault = ConfigurationManager.AppSettings["ServerNameByDefault"];
            if (serverNameByDefault is null)
                return;
            context.Server.Add(Server.DefaultServer).Context.SaveChanges();
        }
    }
}
