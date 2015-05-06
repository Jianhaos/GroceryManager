using Grocery_Master.Common;
using Grocery_Master.GroceryStorageData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Grocery_Master
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StorageListPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public StorageListPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data.
            var group = await GroceryStorageDataSource.GetGroupAsync((string)e.NavigationParameter);
            this.DefaultViewModel["Group"] = group;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            /*var itemId = ((ShoppingListDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
                throw new Exception(resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }*/
            var itemTitle = ((GroceryStorageDataItem)e.ClickedItem).Name;
            if (!Frame.Navigate(typeof(ItemNutritionPage), itemTitle))
            {
                var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
                throw new Exception(resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (itemListView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                itemListView.SelectionMode = ListViewSelectionMode.None;
                itemListView.IsItemClickEnabled = true;
            }
            else
            {
                itemListView.SelectionMode = ListViewSelectionMode.Multiple;
                itemListView.IsItemClickEnabled = false;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var items = itemListView.SelectedItems;
            List<GroceryStorageDataItem> tempItems = new List<GroceryStorageDataItem>();
            foreach (GroceryStorageDataItem item in items)
            {
                tempItems.Add(item);
            }
            //GroceryStorageDataSource.DeleteItemsAsync(items as ObservableCollection<GroceryStorageDataItem>);
            //ObservableCollection<GroceryStorageDataItem> items = itemListView.SelectedItems as ObservableCollection<GroceryStorageDataItem>;
            GroceryStorageDataSource.DeleteItemsAsync(tempItems);
        }
    }
}
