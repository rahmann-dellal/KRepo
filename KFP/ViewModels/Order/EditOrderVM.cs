 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.Services;

namespace KFP.ViewModels
{
    //used in the UI to bring the add-on into view
    public delegate void OnAddedAddonDelegate(OrderItemListElement element);
    public partial class EditOrderVM : KioberViewModelBase
    {
        private AppDataService _appDataService;
        public OnAddedAddonDelegate? onAddedAddonDelegate { get; set; } = null!; //used to bring into view the added element
        public Order order { get; set; } = new();
        public Currency Currency { get; set; } = new();
        public double orderTotalPrice {
            get
            {
                double total = 0;
                foreach (var item in OrderItemElements)
                {
                    total += item.orderItem.TotalPrice;
                    if (item.AddOnItemElements?.Count > 0)
                    {
                        foreach (var addon in item.AddOnItemElements)
                        {
                            total += addon.orderItem.TotalPrice;
                        }
                    }
                }
                return total;
            }
        }

        private OrderItemListElement? _selectedOrderItemElement;
        public OrderItemListElement? selectedOrderItemElement {
            get{
                return _selectedOrderItemElement;
            }
            set
            {
                _selectedOrderItemElement = value;
                OnPropertyChanged(nameof(selectedOrderItemElement));
                //SetProperty(ref _selectedOrderItemElement, value);
                OnOrderItemElementSelected();

                addQuantityCommand?.NotifyCanExecuteChanged();
                reduceQuantityCommand?.NotifyCanExecuteChanged();
                removeOrderItemCommand?.NotifyCanExecuteChanged();
            } 
        }

        public bool isListEmpty
        {
            get
            {
                return OrderItemElements.Count == 0;
            }
        }

        public ObservableCollection<OrderItemListElement> OrderItemElements { get; set; } = new();

        public EditOrderVM(AppDataService appDataService)
        {
            order = new Order();
            // Initialize the order with a new list of order items
            OrderItemElements.CollectionChanged += (s, e) =>
            {
                // Notify the UI that the list has changed
                OnPropertyChanged(nameof(isListEmpty));
                OnPropertyChanged(nameof(orderTotalPrice));
            };
            _appDataService = appDataService;
            Currency = appDataService.Currency;
        }
        public void OnMenuItemSelected(MenuItem menuItem)
        {
           if(menuItem != null)
            {
                if (selectedOrderItemElement != null && selectedOrderItemElement.isAddingAddons && menuItem.MenuItemType != MenuItemType.Main)
                {
                    selectedOrderItemElement.AddAddon(menuItem);
                }
                else
                {
                    OrderItemListElement element = new OrderItemListElement(this);
                    OrderItem item = new OrderItem();
                    item.MenuItem = menuItem;
                    item.MenuItemName = menuItem.ItemName;
                    item.UnitPrice = menuItem.SalePrice;
                    item.Quantity = 1;
                    order.OrderItems.Add(item);
                    element.orderItem = item;
                    OrderItemElements.Add(element);
                    selectedOrderItemElement = element;
                }
                clearOrderCommand?.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(orderTotalPrice));
            }
        }

        public void OnOrderItemElementSelected()
        {
            if(selectedOrderItemElement != null)
            {
                if (!selectedOrderItemElement.isAddingAddons) {
                    OrderItemElements.Where(x => x != selectedOrderItemElement).ToList().ForEach(x => x.isAddingAddons = false);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(canAddQuantity))]
        public void AddQuantity()
        {
            if (selectedOrderItemElement != null)
            {
                selectedOrderItemElement.orderItem.Quantity++;
                if (selectedOrderItemElement.AddOnItemElements.Count > 0)
                {
                    selectedOrderItemElement.AddOnItemElements.ToList().ForEach(x => {
                        if (x.orderItem.Quantity < 99)
                        {
                            x.orderItem.Quantity++;
                        }
                    });
                }
                reduceQuantityCommand?.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(orderTotalPrice));
            }
        }
        public bool canAddQuantity()
        {
            return selectedOrderItemElement != null && selectedOrderItemElement.orderItem.Quantity < 99;
        }

        [RelayCommand(CanExecute = nameof(canReduceQuantity))]
        public void ReduceQuantity()
        {
            if (selectedOrderItemElement != null && selectedOrderItemElement.orderItem.Quantity > 0)
            {
                selectedOrderItemElement.orderItem.Quantity--;
                if (selectedOrderItemElement.orderItem.Quantity == 0)
                {
                    OrderItemElements.Remove(selectedOrderItemElement);
                    selectedOrderItemElement = null;
                }
                if(selectedOrderItemElement.AddOnItemElements.Count > 0)
                {
                    selectedOrderItemElement.AddOnItemElements.ToList().ForEach(x => { 
                        if (x.orderItem.Quantity > 0) 
                        {
                            x.orderItem.Quantity--;
                        } 
                    });
                }
                reduceQuantityCommand?.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(orderTotalPrice));
            }
        }

        [RelayCommand(CanExecute = nameof(canRemoveOrderItem))]
        public void RemoveOrderItem()
        {
            if (selectedOrderItemElement != null)
            {
                if(selectedOrderItemElement.IsAddOn)
                {
                    selectedOrderItemElement.ParentOrderItemElement?.orderItem.AddOns.Remove(selectedOrderItemElement.orderItem);
                    selectedOrderItemElement.ParentOrderItemElement?.AddOnItemElements.Remove(selectedOrderItemElement);
                }
                else
                {
                    order.OrderItems.Remove(selectedOrderItemElement.orderItem);
                    OrderItemElements.Remove(selectedOrderItemElement);                    
                }                
                selectedOrderItemElement = null;
                OnPropertyChanged(nameof(orderTotalPrice));
                clearOrderCommand?.NotifyCanExecuteChanged();
            }
        }

        public bool canRemoveOrderItem()
        {
            return selectedOrderItemElement != null;
        }

        public bool canReduceQuantity()
        {
            return selectedOrderItemElement != null && selectedOrderItemElement.orderItem.Quantity > 1;
        }

        [RelayCommand(CanExecute = nameof(canClearOrder))]
        public void ClearOrder()
        {
            order.OrderItems.Clear();
            OrderItemElements.Clear();
            selectedOrderItemElement = null;
            clearOrderCommand?.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(orderTotalPrice));
        }

        public bool canClearOrder()
        {
            return OrderItemElements.Count > 0;
        }
    }

    public partial class OrderItemListElement : ObservableObject {
        public OrderItem orderItem { get; set; }
        public bool IsAddOn => ParentOrderItemElement != null;
        public OrderItemListElement? ParentOrderItemElement { get; set; }
        public ObservableCollection<OrderItemListElement> AddOnItemElements { get; set; } = new(); 

        private bool _isAddingAddons;
        public bool isAddingAddons
        {
            get
            {
                return _isAddingAddons;
            }
            set
            {
                SetProperty(ref _isAddingAddons, value);
                OnPropertyChanged(nameof(canSelectToAddAddons));
                selectToAddAddonsCommand.NotifyCanExecuteChanged();
                unselectToAddAddonsCommand.NotifyCanExecuteChanged();
            }
        }
        public bool canSelectToAddAddons
        {
            get
            {
                return !IsAddOn && !isAddingAddons && orderItem.MenuItem.MenuItemType != MenuItemType.Addon;
            }
        }
        private EditOrderVM _orderVM;
        public OrderItemListElement(EditOrderVM orderVM)
        {
            _orderVM = orderVM;
        }

        [RelayCommand(CanExecute = nameof(canSelectToAddAddons))]
        public void selectToAddAddons()
        {
            if (!IsAddOn)
            {
                isAddingAddons = true;
                _orderVM.selectedOrderItemElement = this;
                _orderVM.OrderItemElements.Where(x => x != this).ToList().ForEach(x => x.isAddingAddons = false);
            }
        }


        [RelayCommand(CanExecute = nameof(canUnselectToAddAddons))]
        public void unselectToAddAddons()
        {
            if (!IsAddOn)
            {
                isAddingAddons = false;
            }
        }
        public bool canUnselectToAddAddons()
        {
            return !IsAddOn && isAddingAddons;
        }

        public void AddAddon(MenuItem menuItem)
        {
            if (!this.IsAddOn) {
                OrderItemListElement element;
                // Check if the menu item is already an add-on
                if (this.AddOnItemElements.Any(x => x.orderItem.MenuItemId == menuItem.Id))
                {
                    element = this.AddOnItemElements.FirstOrDefault(x => x.orderItem.MenuItemId == menuItem.Id);
                    element.orderItem.Quantity++;
                }
                else
                {
                    element = new OrderItemListElement(_orderVM);
                    OrderItem item = new OrderItem();
                    item.MenuItem = menuItem;
                    item.MenuItemId = menuItem.Id;
                    item.MenuItemName = menuItem.ItemName;
                    item.UnitPrice = menuItem.SalePrice;
                    item.Quantity = this.orderItem.Quantity;
                    item.ParentOrderItem = this.orderItem;
                    item.ParentOrderItemId = this.orderItem.Id;
                    this.orderItem.AddOns.Add(item);
                    element.orderItem = item;
                    element.ParentOrderItemElement = this;
                    this.AddOnItemElements.Add(element);
                }
                _orderVM.onAddedAddonDelegate?.Invoke(element);
            }
        }
    }
}
