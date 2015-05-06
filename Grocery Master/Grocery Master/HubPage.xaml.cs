using Grocery_Master.Common;
using Grocery_Master.FavoriteRecipeData;
using Grocery_Master.GroceryNutritionData;
using Grocery_Master.GroceryStorageData;
using Grocery_Master.NutritionCategoryData;
using Grocery_Master.ShoppingListData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Grocery_Master
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

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

            await createJsonAsync("ms-appx:///DataModel/ShoppingListData.json", "ShoppingListData.json");
            await createJsonAsync("ms-appx:///DataModel/GroceryStorageData.json", "GroceryStorageData.json");
            await createJsonAsync("ms-appx:///DataModel/CategoryData.json", "CategoryData.json");

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            /*var sampleDataGroups = await GroceryDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;*/

            var shoppingListDataGroups = await ShoppingListDataSource.GetGroupsAsync();
            this.DefaultViewModel["ShoppingList"] = shoppingListDataGroups;

            var groceryStorageDataGroups = await GroceryStorageDataSource.GetGroupsAsync();
            this.DefaultViewModel["GroceryStorage"] = groceryStorageDataGroups;

            var nutritionCategoryDataGroups = await NutritionCategoryDataSource.GetItemsAsync();
            this.DefaultViewModel["NutritionCategory"] = nutritionCategoryDataGroups;

            await GroceryNutritionDataSource.GetItemAsync("");
            await FavoriteRecipeDataSource.GetItemsAsync();
        }

        private async Task createJsonAsync(string DATAPATH, string JSONFILENAME)
        {
            try
            {
                Uri dataUri = new Uri(DATAPATH);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                await file.CopyAsync(ApplicationData.Current.LocalFolder, JSONFILENAME, NameCollisionOption.FailIfExists);
            }
            catch (System.Exception e)
            {

            }
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
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            /*var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }*/
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            /*var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }*/
        }

        private void ShoppingListSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((ShoppingListDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ShoppingListPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void StorageListSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((GroceryStorageDataGroup)e.ClickedItem).Category;
            if (!Frame.Navigate(typeof(StorageListPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void NutritionFactSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var key = ((NutritionCategoryDataItem)e.ClickedItem).Key;

            if (!Frame.Navigate(typeof(NutritionAnalysisPage), key))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }

        }

        private void Recommand_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(RecommandationPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void SearchRecipe_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(RecipeSearchPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void Favorite_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(FavoritePage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(NewShoppingListPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
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
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
