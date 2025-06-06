using System.Windows;
using Microsoft.EntityFrameworkCore;
using GlassGrid.Models;
using GlassGrid.Models.Data;
using GlassGrid.ViewModels;
using GlassGrid.Views;

namespace GlassGrid
{
    public partial class App : Application
    {
        public GamePlayViewModel GamePlayVM { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Tworzenie kontekstu i serwisów
            var optionsBuilder = new DbContextOptionsBuilder<GlassGridContext>();
            optionsBuilder.UseSqlServer("Data Source=Rodion;Initial Catalog=GlassGridDb;Integrated Security=true;TrustServerCertificate=true");
            
            var dbContext = new GlassGridContext(optionsBuilder.Options);
            dbContext.Database.EnsureCreated();

            var userService = new EfUserService(dbContext);

            // Tworzymy VM
            GamePlayVM = new GamePlayViewModel(userService, dbContext);

            // Tworzymy okno i ustawiamy DataContext na VM
            var mainWindow = new MainWindow();
            mainWindow.DataContext = GamePlayVM;
            mainWindow.Show();
        }
    }

}
