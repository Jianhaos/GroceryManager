using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.CategoryData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class CategoryDataItem
    {
        public CategoryDataItem(String name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// CategoryDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class CategoryDataSource
    {
        private static CategoryDataSource _CategoryDataSource = new CategoryDataSource();

        private ObservableCollection<CategoryDataItem> _items = new ObservableCollection<CategoryDataItem>();
        public ObservableCollection<CategoryDataItem> Items
        {
            get { return this._items; }
        }

        public static async Task<ObservableCollection<CategoryDataItem>> GetItemAsync(string foodName)
        {
            await _CategoryDataSource.GetCategoryDataAsync(foodName);
            // Simple linear search is acceptable for small data sets
            /*var matches = _CategoryDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.Item_id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;*/
            return _CategoryDataSource.Items;
        }

        private async Task GetCategoryDataAsync(string foodName)
        {
            this.Items.Clear();
            //if (this._groups.Count != 0)
            //return;

            //Intelligent category
            /*var client = new HttpClient();
            string reqUri = string.Format("http://usmangou.com/categoryAPI?food={0}", foodName);
            var uri = new Uri(reqUri);
            var jsonText = await client.GetStringAsync(uri);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["result"].GetObject()["Items"].GetArray();*/

            //Uri dataUri = new Uri("ms-appx:///DataModel/CategoryData.json");
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            //string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["result"].GetObject()["Items"].GetArray();
            //jsonObject = JsonObject.Parse(jsonObject["result"]);
            //JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue itemValue in jsonArray)
            {
                JsonObject itemObject = itemValue.GetObject();
                CategoryDataItem item = new CategoryDataItem(itemObject["name"].GetString());

                this.Items.Add(item);
            }
        }
    }
}