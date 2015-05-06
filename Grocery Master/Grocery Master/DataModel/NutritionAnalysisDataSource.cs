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
using Grocery_Master.GroceryNutritionData;
using System.Reflection;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.NutritionAnalysisData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class NutritionAnalysisDataItem
    {
        public NutritionAnalysisDataItem(String name, double value, String uom)
        {
            this.Name = name;
            this.Value = value;
            this.Uom = uom;

        }
        public string Name { get; private set; }
        public double Value { get; private set; }
        public string Uom { get; private set; }


        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// NutritionAnalysisDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class NutritionAnalysisDataSource
    {
        private static NutritionAnalysisDataSource _NutritionAnalysisDataSource = new NutritionAnalysisDataSource();

        private ObservableCollection<NutritionAnalysisDataItem> _items = new ObservableCollection<NutritionAnalysisDataItem>();
        public ObservableCollection<NutritionAnalysisDataItem> Items
        {
            get { return this._items; }
        }



        public static IEnumerable<NutritionAnalysisDataItem> GetItems(string key)
        {
            _NutritionAnalysisDataSource.GetNutritionAnalysisData(key);

            return _NutritionAnalysisDataSource.Items;
        }

        /*public static async Task<NutritionAnalysisDataItem> GetItemAsync(string key)
        {
            await _NutritionAnalysisDataSource.GetNutritionAnalysisDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _NutritionAnalysisDataSource.Items.Where((item) => item.Key.Equals(key));
            if (matches.Count() == 1) return matches.First();
            return null;
        }*/

        private void GetNutritionAnalysisData(string key)
        {
            this.Items.Clear();
            List<GroceryNutritionDataItem> items = GroceryNutritionDataSource.GetAllItem(key);

            foreach (GroceryNutritionDataItem item in items)
            {

                NutritionAnalysisDataItem newItem = new NutritionAnalysisDataItem(item.Name,
                                                            ((ItemUnit)((item.GetType().GetRuntimeProperty(key)).GetValue(item, null))).Value, ((ItemUnit)((item.GetType().GetRuntimeProperty(key)).GetValue(item, null))).Value.ToString() + ((ItemUnit)((item.GetType().GetRuntimeProperty(key)).GetValue(item, null))).Uom);

                this.Items.Add(newItem);
            }
        }
    }
}