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
using System.Runtime.Serialization;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Grocery_Master.ShoppingListData
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    [DataContract]
    public class ShoppingListDataItem
    {
        public ShoppingListDataItem(String uniqueId, String name, String category)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
            this.Category = category;
        }
        [DataMember]
        public string UniqueId { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Category { get; private set; }

        /*public override string ToString()
        {
            return this.Name;
        }*/
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    [DataContract]
    public class ShoppingListDataGroup
    {
        public ShoppingListDataGroup(String uniqueId, String title, String date, String store)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Date = date;
            this.Store = store;
            this.Items = new ObservableCollection<ShoppingListDataItem>();
        }
        [DataMember]
        public string UniqueId { get; private set; }
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Date { get; private set; }
        [DataMember]
        public string Store { get; private set; }
        [DataMember]
        public ObservableCollection<ShoppingListDataItem> Items { get; private set; }

        /*public override string ToString()
        {
            return this.Title;
        }*/
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// ShoppingListDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class ShoppingListDataSource
    {
        private static String JSONFILENAME = "ShoppingListData.json";
        private static String settingValue = "shoppingList";
        private static ShoppingListDataSource _ShoppingListDataSource = new ShoppingListDataSource();

        private ObservableCollection<ShoppingListDataGroup> _groups = new ObservableCollection<ShoppingListDataGroup>();
        public ObservableCollection<ShoppingListDataGroup> Groups
        {
            get { return this._groups; }
        }

        private ShoppingListDataGroup _newGroup = new ShoppingListDataGroup("", "", "", "");
        public ShoppingListDataGroup NewGroup
        {
            get { return this._newGroup; }
        }

        public static ShoppingListDataGroup GetNewGroup()
        {
            return _ShoppingListDataSource.NewGroup;
        }

        public static void AddtoGroup(String uniqueId, String name, String category)
        {
            _ShoppingListDataSource.NewGroup.Items.Add(new ShoppingListDataItem(uniqueId, name, category));
        }

        public static async void AddtoGroups(String uniqueId, String title, String date, String store, ObservableCollection<ShoppingListDataItem> items)
        {
            ShoppingListDataGroup group = new ShoppingListDataGroup(uniqueId, date + " at " + store, date, store);
            foreach (ShoppingListDataItem item in items)
                group.Items.Add(item);
            _ShoppingListDataSource.Groups.Add(group);
            FileHelper fh = new FileHelper();
            await fh.saveShoppingListDataAsync(JSONFILENAME, _ShoppingListDataSource.Groups);
        }

        public static async void DeleteFromGroups(String uniqueId)
        {
            var matches = _ShoppingListDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
                _ShoppingListDataSource.Groups.Remove(matches.First()); 
            
            FileHelper fh = new FileHelper();
            await fh.saveShoppingListDataAsync(JSONFILENAME, _ShoppingListDataSource.Groups);
        }

        public static async Task<IEnumerable<ShoppingListDataGroup>> GetGroupsAsync()
        {
            await _ShoppingListDataSource.GetShoppingListDataAsync();

            return _ShoppingListDataSource.Groups;
        }

        public static async Task<ShoppingListDataGroup> GetGroupAsync(string uniqueId)
        {
            await _ShoppingListDataSource.GetShoppingListDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _ShoppingListDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<ShoppingListDataItem> GetItemAsync(string uniqueId)
        {
            await _ShoppingListDataSource.GetShoppingListDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _ShoppingListDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetShoppingListDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            FileHelper fh = new FileHelper();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool value = (bool)localSettings.Values[settingValue];

            if (!value)
            {
                List<ShoppingListDataGroup> groups = await fh.deserializeShoppingListJsonAsync(JSONFILENAME);
                foreach (ShoppingListDataGroup group in groups)
                {
                    this.Groups.Add(group);
                }
            }
            else
            {
                string jsonText = await fh.readJsonAsync(JSONFILENAME);
                JsonObject jsonObject = JsonObject.Parse(jsonText);
                JsonArray jsonArray = jsonObject["Groups"].GetArray();

                foreach (JsonValue groupValue in jsonArray)
                {
                    JsonObject groupObject = groupValue.GetObject();
                    ShoppingListDataGroup group = new ShoppingListDataGroup(groupObject["UniqueId"].GetString(),
                                                                groupObject["Title"].GetString(),
                                                                groupObject["Date"].GetString(),
                                                                groupObject["Store"].GetString());

                    foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                    {
                        JsonObject itemObject = itemValue.GetObject();
                        group.Items.Add(new ShoppingListDataItem(itemObject["UniqueId"].GetString(),
                                                           itemObject["Name"].GetString(),
                                                           itemObject["Category"].GetString()));
                    }
                    this.Groups.Add(group);

                }
            }

        }
    }
}