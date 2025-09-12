using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using KFP.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditMenuItemPage : Page
    {
        private MenuItemVM viewModel;
        public EditMenuItemPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var parameters = e.Parameter as List<Object>;
                if (parameters != null && parameters.Count > 0)
                {
                    viewModel = Ioc.Default.GetService<EditMenuItemVM>();
                    int menuItemID = (int)parameters.FirstOrDefault();
                    (viewModel as EditMenuItemVM).Load(menuItemID, OpenFileDialogAndUploadImageAsync, ShowConfirmSaveDialog);
                }
            }
            else
            {
                viewModel = Ioc.Default.GetService<AddMenuItemVM>();
                viewModel.showFileDialogAndGetFileUri = OpenFileDialogAndUploadImageAsync;
                viewModel.showConfirmSaveDialog = ShowConfirmSaveDialog;
            }
            viewModel.assignedCategories.CollectionChanged += OnAssignedCategoriesCollectionChanged;
            NothingTextBlock.Visibility = viewModel.assignedCategories.Count > 0 ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
        }

        // maybe betterdone through Binding ??
        private void OnAssignedCategoriesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (viewModel.assignedCategories.Count > 0)
            {
                NothingTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                NothingTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
        }

        public async Task<string> OpenFileDialogAndUploadImageAsync()
        {
            var picker = new FileOpenPicker();

            // Make the picker work in WinUI 3
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Filter for image files
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");

            StorageFile file = await picker.PickSingleFileAsync();
            if(file == null)
            {
                return null;
            }
            return file.Path;
        }

        public async Task<bool> ShowConfirmSaveDialog()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("EditMenuItemPage_save_changes");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
