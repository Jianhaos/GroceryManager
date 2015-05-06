using Grocery_Master.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
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

namespace Grocery_Master.GroceryNutritionData
{
    [DataContract]
    public class ItemUnit
    {
        public ItemUnit(double value, String desc, String uom)
        {
            this.Value = value;
            this.Desc = desc;
            this.Uom = uom;
        }
        [DataMember]
        public double Value { get; private set; }
        [DataMember]
        public string Desc { get; private set; }
        [DataMember]
        public string Uom { get; private set; }
    }
    /// <summary>
    /// Generic item data model.
    /// </summary>
    [DataContract]
    public class GroceryNutritionDataItem
    {
        public GroceryNutritionDataItem(String name, double weight, ItemUnit water, ItemUnit enerc_kcal, ItemUnit procnt, ItemUnit fat, ItemUnit chocdf, ItemUnit fibtg, ItemUnit sugar, ItemUnit vita_rae, ItemUnit vitb6a, ItemUnit vitab12, ItemUnit vitc)
        {
            this.Name = name;
            this.WEIGHT = weight;
            this.WATER = water;
            this.ENERC_KCAL = enerc_kcal;
            this.PROCNT = procnt;
            this.FAT = fat;
            this.CHOCDF = chocdf;
            this.FIBTG = fibtg;
            this.SUGAR = sugar;
            this.VITA_RAE = vita_rae;
            this.VITB6A = vitb6a;
            this.VITB12 = vitab12;
            this.VITC = vitc;
        }
        [DataMember]
        public String Name { get; private set; }
        [DataMember]
        public double WEIGHT { get; private set; }
        [DataMember]
        public ItemUnit WATER { get; private set; }
        [DataMember]
        public ItemUnit ENERC_KCAL { get; private set; }
        [DataMember]
        public ItemUnit PROCNT { get; private set; }
        [DataMember]
        public ItemUnit FAT { get; private set; }
        [DataMember]
        public ItemUnit CHOCDF { get; private set; }
        [DataMember]
        public ItemUnit FIBTG { get; private set; }
        [DataMember]
        public ItemUnit SUGAR { get; private set; }
        [DataMember]
        public ItemUnit VITA_RAE { get; private set; }
        [DataMember]
        public ItemUnit VITB6A { get; private set; }
        [DataMember]
        public ItemUnit VITB12 { get; private set; }
        [DataMember]
        public ItemUnit VITC { get; private set; }


        /*public override string ToString()
        {
            return this.Item_name;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// GroceryNutritionDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class GroceryNutritionDataSource
    {
        private bool isFirst = true;
        private static String JSONFILENAME = "GroceryNutritionData.json";
        private static GroceryNutritionDataSource _GroceryNutritionDataSource = new GroceryNutritionDataSource();

        private ObservableCollection<GroceryNutritionDataItem> _items = new ObservableCollection<GroceryNutritionDataItem>();
        public ObservableCollection<GroceryNutritionDataItem> Items
        {
            get { return this._items; }
        }

        public static async Task<GroceryNutritionDataItem> GetItemAsync(string foodName)
        {
            await _GroceryNutritionDataSource.GetGroceryNutritionDataAsync(foodName);
            // Simple linear search is acceptable for small data sets
            var matches = _GroceryNutritionDataSource.Items.Where((item) => item.Name.Equals(foodName));
            if (matches.Count() == 1) return matches.First();
            return null;
            //return _GroceryNutritionDataSource.Items[0];
        }

        public static List<GroceryNutritionDataItem> GetAllItem(string key)
        {
            //return _GroceryNutritionDataSource.Items.OrderByDescending((item) => item.WATER.Value).ToList<GroceryNutritionDataItem>();
            return _GroceryNutritionDataSource.Items.OrderByDescending((item) => ((ItemUnit)((item.GetType().GetRuntimeProperty(key)).GetValue(item, null))).Value).ToList<GroceryNutritionDataItem>();
        }

        private async Task GetGroceryNutritionDataAsync(string foodName)
        {
            //this.Items.Clear();
            if (this._items.Count != 0)
            {
                var matches = this.Items.Where((item) => item.Name.Equals(foodName));
                if (matches.Count() == 1)
                    return;
            }
            FileHelper fh = new FileHelper();

            if (isFirst)
            {
                isFirst = false;
                List<GroceryNutritionDataItem> items = await fh.deserializeGroceryNutritionJsonAsync(JSONFILENAME);
                foreach (GroceryNutritionDataItem item in items)
                {
                    this.Items.Add(item);
                }

                return;

                /*if (this._items.Count != 0)
                {

                    var matches = this.Items.Where((item) => item.Name.Equals(foodName));
                    if (matches.Count() == 1)
                        return;
                }*/
            }
            else if (foodName == "")
                return;

            var client = new HttpClient();
            string reqUri = string.Format("http://usmangou.com/nutritionAPI?food={0}", foodName);
            var uri = new Uri(reqUri);
            var jsonText = await client.GetStringAsync(uri);

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonObject result = jsonObject["result"].GetObject();

            double weight = result["WEIGHT"].GetNumber();
            GroceryNutritionDataItem newItem = new GroceryNutritionDataItem(result["Name"].GetString(), weight,
                new ItemUnit(GetUnitWeight(weight, result["WATER"].GetObject()["value"].GetNumber()), GetShortDesc(result["WATER"].GetObject()["desc"].GetString()), result["WATER"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["ENERC_KCAL"].GetObject()["value"].GetNumber()), GetShortDesc(result["ENERC_KCAL"].GetObject()["desc"].GetString()), result["ENERC_KCAL"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["PROCNT"].GetObject()["value"].GetNumber()), GetShortDesc(result["PROCNT"].GetObject()["desc"].GetString()), result["PROCNT"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["FAT"].GetObject()["value"].GetNumber()), GetShortDesc(result["FAT"].GetObject()["desc"].GetString()), result["FAT"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["CHOCDF"].GetObject()["value"].GetNumber()), GetShortDesc(result["CHOCDF"].GetObject()["desc"].GetString()), result["CHOCDF"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["FIBTG"].GetObject()["value"].GetNumber()), GetShortDesc(result["FIBTG"].GetObject()["desc"].GetString()), result["FIBTG"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["SUGAR"].GetObject()["value"].GetNumber()), GetShortDesc(result["SUGAR"].GetObject()["desc"].GetString()), result["SUGAR"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["VITA_RAE"].GetObject()["value"].GetNumber()), GetShortDesc(result["VITA_RAE"].GetObject()["desc"].GetString()), result["VITA_RAE"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["VITB6A"].GetObject()["value"].GetNumber()), GetShortDesc(result["VITB6A"].GetObject()["desc"].GetString()), result["VITB6A"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["VITB12"].GetObject()["value"].GetNumber()), GetShortDesc(result["VITB12"].GetObject()["desc"].GetString()), result["VITB12"].GetObject()["uom"].GetString()),
                new ItemUnit(GetUnitWeight(weight, result["VITC"].GetObject()["value"].GetNumber()), GetShortDesc(result["VITC"].GetObject()["desc"].GetString()), result["VITC"].GetObject()["uom"].GetString()));
            this.Items.Add(newItem);

            await fh.saveGroceryNutritionDataAsync(JSONFILENAME, _GroceryNutritionDataSource.Items);

        }

        private double GetUnitWeight(double unitWeight, double weight)
        {

            return weight / (unitWeight * 0.01);
        }

        private string GetShortDesc(string desc)
        {

            return desc.Contains(',') ? desc.Substring(0, desc.IndexOf(',')) : desc;
        }
    }
}