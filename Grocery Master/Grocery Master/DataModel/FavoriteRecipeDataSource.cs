using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Grocery_Master.Common;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Grocery_Master.ShoppingListData;
using System.Net.Http;
using Grocery_Master.GroceryStorageData;
using Grocery_Master.RecommandationData;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.FavoriteRecipeData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    [DataContract]
    public class FavoriteRecipeDataItem
    {
        
        public FavoriteRecipeDataItem(String url, String name, String image, String[] ingredients)
        {
            this.Url = url;
            this.Name = name;
            this.Image = image;
            this.Ingredients = ingredients;

        }
        [DataMember]
        public string Url { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Image { get; private set; }
        [DataMember]
        public string[] Ingredients { get; private set; }


        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// FavoriteRecipeDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class FavoriteRecipeDataSource
    {
        private static String JSONFILENAME = "FavoriteRecipeData.json";
        private static FavoriteRecipeDataSource _FavoriteRecipeDataSource = new FavoriteRecipeDataSource();

        private ObservableCollection<FavoriteRecipeDataItem> _items = new ObservableCollection<FavoriteRecipeDataItem>();
        public ObservableCollection<FavoriteRecipeDataItem> Items
        {
            get { return this._items; }
        }

        public static async Task<IEnumerable<FavoriteRecipeDataItem>> GetItemsAsync()
        {
            await _FavoriteRecipeDataSource.GetFavoriteRecipeDataAsync();
            return _FavoriteRecipeDataSource.Items;
        }

        public static async void AddItemsAsync(RecommandationDataItem item)
        {

            _FavoriteRecipeDataSource.Items.Add(new FavoriteRecipeDataItem(item.Url, item.Name, item.Image, item.Ingredients));
            FileHelper fh = new FileHelper();
            await fh.saveFavoriteRecipeDataAsync(JSONFILENAME, _FavoriteRecipeDataSource.Items);
        }

        private async Task GetFavoriteRecipeDataAsync()
        {

            if (this._items.Count != 0)
                return;

            FileHelper fh = new FileHelper();
            List<FavoriteRecipeDataItem> items = await fh.deserializeFavoriteRecipeJsonAsync(JSONFILENAME);
            foreach (FavoriteRecipeDataItem item in items)
            {
                this.Items.Add(item);
            }

        }




    }
}