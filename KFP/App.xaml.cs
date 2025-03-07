using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA_Access;
using KFP.Services;
using KFP.Ui;
using KFP.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? m_window;

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
                db.Database.EnsureCreated();
            }

            var services = new ServiceCollection();

            //DB
            services.AddDbContext<KFPContext>();

            //VMs
            services.AddTransient<DisplayUserVM>();
            services.AddTransient<EditUserVM>();

            //Services
            services.AddSingleton<AppDataService>();
            services.AddSingleton<SessionManager>();
            services.AddSingleton<AppState>();
            services.AddSingleton<NavigationService>();


            var serviceProvider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

            m_window = new MainWindow();
            m_window.Activate();
        }
    }
}
