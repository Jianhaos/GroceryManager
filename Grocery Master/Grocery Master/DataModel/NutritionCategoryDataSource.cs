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

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.NutritionCategoryData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class NutritionCategoryDataItem
    {
        public NutritionCategoryDataItem(String name, String key)
        {
            this.Name = name;
            this.Key = key;

        }
        public string Name { get; private set; }
        public string Key { get; private set; }


        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// NutritionCategoryDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class NutritionCategoryDataSource
    {
        private static NutritionCategoryDataSource _NutritionCategoryDataSource = new NutritionCategoryDataSource();

        private ObservableCollection<NutritionCategoryDataItem> _items = new ObservableCollection<NutritionCategoryDataItem>();
        public ObservableCollection<NutritionCategoryDataItem> Items
        {
            get { return this._items; }
        }



        public static async Task<IEnumerable<NutritionCategoryDataItem>> GetItemsAsync()
        {
            await _NutritionCategoryDataSource.GetNutritionCategoryDataAsync();

            return _NutritionCategoryDataSource.Items;
        }

        public static async Task<NutritionCategoryDataItem> GetItemAsync(string key)
        {
            await _NutritionCategoryDataSource.GetNutritionCategoryDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _NutritionCategoryDataSource.Items.Where((item) => item.Key.Equals(key));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetNutritionCategoryDataAsync()
        {
            if (this._items.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/NutritionCategoryData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Items"].GetArray();

            foreach (JsonValue itemValue in jsonArray)
            {
                JsonObject itemObject = itemValue.GetObject();
                NutritionCategoryDataItem item = new NutritionCategoryDataItem(itemObject["Name"].GetString(),
                                                            itemObject["Key"].GetString());

                this.Items.Add(item);
            }

        }
    }
}