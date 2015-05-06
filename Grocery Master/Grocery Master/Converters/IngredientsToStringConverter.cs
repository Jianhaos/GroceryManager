using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Grocery_Master.Converters
{
    class IngredientsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] ingredients = (string[])value;
            string newIngredient = String.Empty;
            foreach (string ingredient in ingredients)
            {
                newIngredient += ingredient+", ";
            }
            newIngredient.Trim(' ');
            newIngredient.Trim(',');
            return newIngredient;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
