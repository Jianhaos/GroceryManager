using Grocery_Master.FavoriteRecipeData;
using Grocery_Master.GroceryNutritionData;
using Grocery_Master.GroceryStorageData;
using Grocery_Master.ShoppingListData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Grocery_Master.Common
{
    class FileHelper
    {
        public async Task<string> readJsonAsync(string JSONFILENAME)
        {

            string content = String.Empty;
            try
            {
                var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);
                using (StreamReader reader = new StreamReader(myStream))
                {
                    //content = await reader.re();
                    content = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                //string dsad = "";
            }
            return content;
        }

        public async Task<List<ShoppingListDataGroup>> deserializeShoppingListJsonAsync(string JSONFILENAME)
        {
            List<ShoppingListDataGroup> myGroup;
            var serializer = new DataContractJsonSerializer(typeof(List<ShoppingListDataGroup>));

            var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

            myGroup = (List<ShoppingListDataGroup>)serializer.ReadObject(myStream);

            return myGroup;
        }

        public async Task<List<GroceryNutritionDataItem>> deserializeGroceryNutritionJsonAsync(string JSONFILENAME)
        {

            List<GroceryNutritionDataItem> myGroup;
            var serializer = new DataContractJsonSerializer(typeof(List<GroceryNutritionDataItem>));
            try
            {
                var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

                myGroup = (List<GroceryNutritionDataItem>)serializer.ReadObject(myStream);
            }
            catch (Exception e) { myGroup = new List<GroceryNutritionDataItem>(); }

            return myGroup;
        }

        public async Task<List<FavoriteRecipeDataItem>> deserializeFavoriteRecipeJsonAsync(string JSONFILENAME)
        {

            List<FavoriteRecipeDataItem> myGroup;
            var serializer = new DataContractJsonSerializer(typeof(List<FavoriteRecipeDataItem>));
            try
            {
                var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

                myGroup = (List<FavoriteRecipeDataItem>)serializer.ReadObject(myStream);
            }
            catch (Exception e) { myGroup = new List<FavoriteRecipeDataItem>(); }

            return myGroup;
        }

        public async Task<List<GroceryStorageDataGroup>> deserializeGroceryStorageJsonAsync(string JSONFILENAME)
        {
            List<GroceryStorageDataGroup> myGroup;
            var serializer = new DataContractJsonSerializer(typeof(List<GroceryStorageDataGroup>));

            var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

            myGroup = (List<GroceryStorageDataGroup>)serializer.ReadObject(myStream);

            return myGroup;
        }

        public async Task saveGroceryStorageDataAsync(string JSONFILENAME, ObservableCollection<GroceryStorageDataGroup> group)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<GroceryStorageDataGroup>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, group);
            }

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool value = (bool)localSettings.Values["groceryStorageFirstTime"];

            if (value)
            {
                localSettings.Values["groceryStorageFirstTime"] = false;
            }
        }

        public async Task saveGroceryNutritionDataAsync(string JSONFILENAME, ObservableCollection<GroceryNutritionDataItem> group)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<GroceryNutritionDataItem>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, group);
            } 
        }

        public async Task saveFavoriteRecipeDataAsync(string JSONFILENAME, ObservableCollection<FavoriteRecipeDataItem> group)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<FavoriteRecipeDataItem>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, group);
            }
        }

        public async Task saveShoppingListDataAsync(string JSONFILENAME, ObservableCollection<ShoppingListDataGroup> group)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<ShoppingListDataGroup>));
            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting))
                {
                    jsonSerializer.WriteObject(stream, group);
                }
            }
            catch (Exception e)
            {
            }
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool value = (bool)localSettings.Values["shoppingListFirstTime"];

            if (value)
            {
                localSettings.Values["shoppingListFirstTime"] = false;
            }
        }
    }
}
