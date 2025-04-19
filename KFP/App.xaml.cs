using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using KFP.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow? MainWindow;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            using (var db = new KFPContext())
            {
                db.Database.Migrate();
                if (!db.AppUsers.Any()) {
                    db.AppUsers.Add(new DATA.AppUser()
                    {
                        UserName = "Admin",
                        Role = DATA.UserRole.Admin,
                        PINHash = "kbTRQoI/fSDF8I32kSLeQ/NfBXqYjZYZ9tMThIXJogM=",
                        avatarCode = 0
                    });
                    db.SaveChanges();
                }
            }

            var services = new ServiceCollection();

            //DB
            services.AddDbContext<KFPContext>();

            //VMs
            services.AddTransient<DisplayUserVM>();
            services.AddTransient<EditUserVM>();
            services.AddTransient<EditMenuItemVM>();
            services.AddTransient<AddMenuItemVM>();
            services.AddTransient<MenuItemListVM>();
            services.AddTransient<ImageConverter>();
            services.AddTransient<FileSystemAccess>();

            //Services
            services.AddSingleton<AppDataService>();
            services.AddSingleton<SessionManager>();
            services.AddSingleton<AppState>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<INavigationService>(s => s.GetRequiredService<NavigationService>());


            var serviceProvider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
