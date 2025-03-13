using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        private KFPContext _dbContext;
        private ObservableCollection<Category> _categories;
        private Category? _selectedCategory = null;
        private Category? selectedCategory { 
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                ShowEditiCategoryPanelCommand.NotifyCanExecuteChanged();
                ShowAddCategoryPanelCommand.NotifyCanExecuteChanged();
                DeleteCategoryCommand.NotifyCanExecuteChanged();
            }
        }
        private string _oldName;

        private Boolean _addingNewCategory = false;
        private Boolean addingNewCategory { get
            {
                return _addingNewCategory;
            }
            set
            {
                _addingNewCategory = value;
                ShowAddCategoryPanelCommand.NotifyCanExecuteChanged();
                ShowEditiCategoryPanelCommand.NotifyCanExecuteChanged();
                deleteCategoryCommand.NotifyCanExecuteChanged();
            }
        }
        private Boolean _EditingExistingCategory = false;

        private Boolean EditingExistingCategory
        {
            get
            {
                return _EditingExistingCategory;
            } set
            {
                _EditingExistingCategory = value;
                ShowAddCategoryPanelCommand.NotifyCanExecuteChanged();
                ShowEditiCategoryPanelCommand.NotifyCanExecuteChanged();
                deleteCategoryCommand.NotifyCanExecuteChanged();
            }
        }

        public CategoriesPage()
        {
            _dbContext = Ioc.Default.GetService<KFPContext>();
            _categories = new ObservableCollection<Category>();
            this.InitializeComponent();
            populateList();
            ShowHideList();
            _categories.CollectionChanged += _categories_CollectionChanged;
            EditingPanel.Visibility = Visibility.Collapsed;
        }
        private void populateList()
        {
            _categories.Clear();
            var categories = _dbContext.Categories.ToList();
            foreach (Category category in categories)
            {
                _categories.Add(category);
            }
        }
        [RelayCommand(CanExecute = nameof(CanEdit))]
        public void ShowEditiCategoryPanel()
        {
            nameErrorBlock.Visibility = Visibility.Collapsed;
            addingNewCategory = false;
            EditingExistingCategory = true;
            CategoriesListView.IsEnabled = false;
            EditingPanel.Visibility = Visibility.Visible;
            name_textbox.Header = StringLocalisationService.getStringWithKey("NewName");
            new_categoty_text_block.Text = StringLocalisationService.getStringWithKey("Editing") + selectedCategory.CategoryName;
            name_textbox.Text = selectedCategory.CategoryName;
            _oldName = selectedCategory.CategoryName;
        }
        [RelayCommand (CanExecute = nameof(CanAdd))]
        public void ShowAddCategoryPanel()
        {
            nameErrorBlock.Visibility = Visibility.Collapsed;
            addingNewCategory = true;
            EditingExistingCategory = false;
            selectedCategory = null;
            CategoriesListView.SelectedItem = null;
            CategoriesListView.IsEnabled = false;
            EditingPanel.Visibility = Visibility.Visible;
            name_textbox.Header = StringLocalisationService.getStringWithKey("Name");
            new_categoty_text_block.Text = StringLocalisationService.getStringWithKey("New_Category");
            name_textbox.Text = "";
        }
        [RelayCommand]
        public void Cancel()
        {
            CategoriesListView.IsEnabled = true;
            addingNewCategory = false;
            EditingExistingCategory = false;
            EditingPanel.Visibility = Visibility.Collapsed;
        }
        [RelayCommand (CanExecute = nameof(canDelete))]
        public void DeleteCategory()
        {
            if(selectedCategory != null)
            {
                _dbContext.Remove(selectedCategory);
                _dbContext.SaveChanges();
                selectedCategory = null;
                populateList();
            }
        }

        [RelayCommand (CanExecute = nameof(canSave))]
        public void Save()
        {
            if (EditingExistingCategory) { 
                var category = _dbContext.Entry(selectedCategory).Entity;
                if (category != null)
                {
                    category.CategoryName = name_textbox.Text;
                }
                _dbContext.SaveChanges();
                CategoriesListView.IsEnabled = true;
                addingNewCategory = false;
                EditingExistingCategory = false;
                EditingPanel.Visibility = Visibility.Collapsed;
            } else
            {
                _dbContext.Categories.Add(new Category() { CategoryName = name_textbox.Text });
                _dbContext.SaveChanges ();
                CategoriesListView.IsEnabled = true;
                addingNewCategory = false;
                EditingExistingCategory = false;
                EditingPanel.Visibility = Visibility.Collapsed;
            }
            populateList();
            name_textbox.Text = string.Empty;
            _oldName = "";
        }
        public bool canSave()
        {
            if (EditingExistingCategory)
            {
                return name_textbox.Text.Length > 0 && name_textbox.Text != _oldName &&
                    !_dbContext.Categories.Where(c => c.CategoryName == name_textbox.Text).Any();
            }
            else
            {
                return name_textbox.Text.Length > 0 && !_dbContext.Categories.Where(c => c.CategoryName == name_textbox.Text).Any();
            }
        }
        public bool CanEdit()
        {
            return _selectedCategory != null && !_EditingExistingCategory && !_addingNewCategory;
        }
        public bool CanAdd()
        {
            return !_EditingExistingCategory && !_addingNewCategory;
        }

        public bool canDelete()
        {
            return _selectedCategory != null && !_EditingExistingCategory && !_addingNewCategory;
        }

        private void _categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ShowHideList();
        }
        private void ShowHideList()
        {
            if (_categories.Count > 0) {
                NothingBorder.Visibility = Visibility.Collapsed;
                listBorder.Visibility = Visibility.Visible;
            } else
            {
                listBorder.Visibility = Visibility.Collapsed;
                NothingBorder.Visibility = Visibility.Visible;
            }
        }

        private void name_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditingExistingCategory && (name_textbox.Text.Length == 0 || name_textbox.Text == _oldName))
            {
                nameErrorBlock.Text = StringLocalisationService.getStringWithKey("Please_provide_new_name");
                nameErrorBlock.Visibility = Visibility.Visible;
            }
            else if (_dbContext.Categories.Where(c => c.CategoryName == name_textbox.Text).Any())
            {
                nameErrorBlock.Text = StringLocalisationService.getStringWithKey("category_with_same_name_exists");
                nameErrorBlock.Visibility = Visibility.Visible;
            }
            else
            {
                nameErrorBlock.Visibility = Visibility.Collapsed;
            }
            SaveCommand.NotifyCanExecuteChanged();
        }
    }
}
