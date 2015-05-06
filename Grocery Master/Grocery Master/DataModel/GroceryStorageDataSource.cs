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

namespace Grocery_Master.GroceryStorageData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    [DataContract]
    public class GroceryStorageDataItem
    {
        public GroceryStorageDataItem(String uniqueId, String name, String date, double expire, string imagePath)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
            this.Date = date;
            this.Expire = expire;
            this.ImagePath = imagePath;
        }
        [DataMember]
        public string UniqueId { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Date { get; private set; }
        [DataMember]
        public double Expire { get; private set; }
        [DataMember]
        public string ImagePath { get; private set; }

        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    [DataContract]
    public class GroceryStorageDataGroup
    {
        public GroceryStorageDataGroup(String uniqueId, String category, double expire, string imagePath, string name, double count)
        {
            this.UniqueId = uniqueId;
            this.Category = category;
            this.Expire = expire;
            this.ImagePath = imagePath;
            this.Name = name;
            this.Count = count;
            this.Items = new ObservableCollection<GroceryStorageDataItem>();
        }
        [DataMember]
        public string UniqueId { get; private set; }
        [DataMember]
        public string Category { get; private set; }
        [DataMember]
        public double Expire { get; private set; }
        [DataMember]
        public string ImagePath { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public double Count { get; private set; }
        [DataMember]
        public ObservableCollection<GroceryStorageDataItem> Items { get; private set; }

        /*public override string ToString()
        {
            return this.Category;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// GroceryStorageDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class GroceryStorageDataSource
    {
        private static String JSONFILENAME = "GroceryStorageData.json";
        private static String settingValue = "groceryStorage";
        private static GroceryStorageDataSource _GroceryStorageDataSource = new GroceryStorageDataSource();

        private ObservableCollection<GroceryStorageDataGroup> _groups = new ObservableCollection<GroceryStorageDataGroup>();
        public ObservableCollection<GroceryStorageDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async void AddtoGroups(String date, ObservableCollection<ShoppingListDataItem> items)
        {
            foreach (GroceryStorageDataGroup group in _GroceryStorageDataSource.Groups)
            {
                foreach (ShoppingListDataItem item in items)
                {
                    if (group.Category == item.Category)
                    {
                        group.Items.Add(new GroceryStorageDataItem(item.UniqueId, item.Name, date, SetImageWidth(date, group.Expire), GetExpireCondition(date, group.Expire)));
                        //items.Remove(item);
                    }
                }
            }

            FileHelper fh = new FileHelper();
            await fh.saveGroceryStorageDataAsync(JSONFILENAME, _GroceryStorageDataSource.Groups);
        }

        public static async Task<IEnumerable<GroceryStorageDataGroup>> GetGroupsAsync()
        {
            await _GroceryStorageDataSource.GetGroceryStorageDataAsync();

            return _GroceryStorageDataSource.Groups;
        }

        public static async Task<GroceryStorageDataGroup> GetGroupAsync(string uniqueId)
        {
            await _GroceryStorageDataSource.GetGroceryStorageDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _GroceryStorageDataSource.Groups.Where((group) => group.Category.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<GroceryStorageDataItem> GetItemAsync(string uniqueId)
        {
            await _GroceryStorageDataSource.GetGroceryStorageDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _GroceryStorageDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async void DeleteItemsAsync(List<GroceryStorageDataItem> items)
        {
            foreach (GroceryStorageDataItem tempItem in items)
            {
                GroceryStorageDataItem item = await GetItemAsync(tempItem.UniqueId);
                foreach (GroceryStorageDataGroup group in _GroceryStorageDataSource.Groups)
                {
                    if (group.Items.Contains(item))
                        group.Items.Remove(item);
                }
            }

            FileHelper fh = new FileHelper();
            await fh.saveGroceryStorageDataAsync(JSONFILENAME, _GroceryStorageDataSource.Groups);
        }

        public static List<string> GetFoods()
        {
            List<string> foods = new List<string>();
            var matches = _GroceryStorageDataSource.Groups.Where((group) => group.Category.Equals("Fruits & Vegetables"));
            if (matches.Count() == 1)
            {
                foreach (GroceryStorageDataItem item in matches.First().Items)
                {
                    foods.Add(item.Name);
                }
            }
            matches = _GroceryStorageDataSource.Groups.Where((group) => group.Category.Equals("Meat & Seafood"));
            if (matches.Count() == 1)
            {
                foreach (GroceryStorageDataItem item in matches.First().Items)
                {
                    foods.Add(item.Name);
                }
            }
            return foods;
        }

        private async Task GetGroceryStorageDataAsync()
        {
            if (this._groups.Count != 0)
                return;


            FileHelper fh = new FileHelper();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool value = (bool)localSettings.Values[settingValue];

            if (!value)
            {
                List<GroceryStorageDataGroup> groups = await fh.deserializeGroceryStorageJsonAsync(JSONFILENAME);
                foreach (GroceryStorageDataGroup group in groups)
                {
                    this.Groups.Add(group);
                }
            }
            else
            {
                string jsonText = await fh.readJsonAsync("GroceryStorageData.json");
                JsonObject jsonObject = JsonObject.Parse(jsonText);
                JsonArray jsonArray = jsonObject["Groups"].GetArray();

                foreach (JsonValue groupValue in jsonArray)
                {
                    JsonObject groupObject = groupValue.GetObject();
                    GroceryStorageDataGroup group = new GroceryStorageDataGroup(groupObject["UniqueId"].GetString(),
                                                                groupObject["Category"].GetString(),
                                                                groupObject["Expire"].GetNumber(),
                                                                groupObject["ImagePath"].GetString(),
                                                                groupObject["Name"].GetString(),
                                                                groupObject["Count"].GetNumber());

                    foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                    {
                        JsonObject itemObject = itemValue.GetObject();
                        group.Items.Add(new GroceryStorageDataItem(itemObject["UniqueId"].GetString(),
                                                           itemObject["Name"].GetString(),
                                                           itemObject["Date"].GetString(),
                                                           SetImageWidth(itemObject["Date"].GetString(), groupObject["Expire"].GetNumber()), GetExpireCondition(itemObject["Date"].GetString(), groupObject["Expire"].GetNumber())));
                    }
                    this.Groups.Add(group);
                }
            }
        }

        private static string GetExpireCondition(string date, double expire)
        {
            double dif = (DateTime.Now - System.Convert.ToDateTime(date)).TotalDays;
            string imagePath = "Assets/";
            if (dif / expire < 1.0 / 3.0)
                imagePath += "Green.png";
            else if (dif / expire >= 1.0 / 3.0 && dif / expire < 1.0 / 3.0)
                imagePath += "Yellow.png";
            else if (dif / expire >= 1.0 / 3.0 && dif / expire < 1.0)
                imagePath += "Orange.png";
            else
                imagePath += "Red.png";
            return imagePath;
        }

        private static double SetImageWidth(string date, double expire)
        {
            double width = 0;
            double dif = (DateTime.Now - System.Convert.ToDateTime(date)).TotalDays;
            if (dif / expire < 1.0 / 3.0)
                width = 100;
            else if (dif / expire >= 1.0 / 3.0 && dif / expire < 1.0 / 3.0)
                width = 80;
            else if (dif / expire >= 1.0 / 3.0 && dif / expire < 1.0)
                width = 60;
            else
                width = 40;
            return width;
        }


    }
}