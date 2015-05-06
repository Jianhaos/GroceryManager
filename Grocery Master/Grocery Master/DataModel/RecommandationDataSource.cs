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

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.RecommandationData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class RecommandationDataItem
    {
        public RecommandationDataItem(String url, String name, String image, String[] ingredients)
        {
            this.Url = url;
            this.Name = name;
            this.Image = image;
            this.Ingredients = ingredients;

        }
        public string Url { get; private set; }
        public string Name { get; private set; }
        public string Image { get; private set; }
        public string[] Ingredients { get; private set; }


        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// RecommandationDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class RecommandationDataSource
    {
        private static RecommandationDataSource _RecommandationDataSource = new RecommandationDataSource();

        private ObservableCollection<RecommandationDataItem> _items = new ObservableCollection<RecommandationDataItem>();
        public ObservableCollection<RecommandationDataItem> Items
        {
            get { return this._items; }
        }

        public static async Task<IEnumerable<RecommandationDataItem>> GetItemsAsync()
        {
            await _RecommandationDataSource.GetRecommandationDataAsync();

            return _RecommandationDataSource.Items;
        }


        public static async Task<IEnumerable<RecommandationDataItem>> GetItemsAsync(string foods)
        {
            await _RecommandationDataSource.GetRecommandationDataAsync(foods);

            return _RecommandationDataSource.Items;
        }

        private async Task GetRecommandationDataAsync()
        {

            /*if (this._items.Count != 0)
                return;*/
            var client = new HttpClient();
            List<string> foods = GroceryStorageDataSource.GetFoods();
            string ingredients = String.Empty;

            string reqUri = string.Format("http://usmangou.com/recipeAPI?food=" + string.Join(",", foods.ToArray()));
            var uri = new Uri(reqUri);
            var jsonText = await client.GetStringAsync(uri);

            this.Items.Clear();

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["result"].GetArray();

            foreach (JsonValue itemValue in jsonArray)
            {
                JsonObject itemObject = itemValue.GetObject();
                RecommandationDataItem item = new RecommandationDataItem(itemObject["Url"].GetString(),
                                                            itemObject["Name"].GetString(), itemObject["Image"].GetString(), itemObject["Ingredients"].GetString().Split(','));

                this.Items.Add(item);
            }

        }

        private async Task GetRecommandationDataAsync(string foods)
        {

            /*if (this._items.Count != 0)
                return;*/
            var client = new HttpClient();

            string reqUri = string.Format("http://usmangou.com/searchAPI?food=" + foods);
            var uri = new Uri(reqUri);
            var jsonText = await client.GetStringAsync(uri);

            this.Items.Clear();

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["result"].GetArray();

            foreach (JsonValue itemValue in jsonArray)
            {
                JsonObject itemObject = itemValue.GetObject();
                RecommandationDataItem item = new RecommandationDataItem(itemObject["Url"].GetString(),
                                                            itemObject["Name"].GetString(), itemObject["Image"].GetString(), itemObject["Ingredients"].GetString().Split(','));

                this.Items.Add(item);
            }

        }
    }
}