using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Vanrise.CommonLibrary
{
    public static class ExtensionMethods
    {
        public static int FindMissing(this List<int> list)
        {
            // Sorting the list
            list.Sort();

            // First number of the list
            var firstNumber = list.First();

            // Last number of the list
            var lastNumber = list.Last();

            // Range that contains all numbers in the interval
            // [ firstNumber, lastNumber ]
            var range = Enumerable.Range(firstNumber, lastNumber - firstNumber+1);

            // Getting the set difference
            int missingNumber= lastNumber +1;

            foreach (var i in range)
            {
                if (!list.Contains(i))
                {
                    missingNumber = i;
                    break;
                }
            }

            return missingNumber;
        }

        public static bool ContainInsensitive(this string target, string value)
        {
            return target.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string RemoveSpaces(this string target)
        {
            return target.Replace(" ", "") ;
        }

        public static void FocusAfterLastChar(this TextBox txt)
        {
            if (txt.Page.ClientScript.IsStartupScriptRegistered(txt.Page.GetType(), "Select-" + txt.ClientID) == false)
            {
                txt.Page.ClientScript.RegisterStartupScript(txt.Page.GetType(),
                                                 "Select-" + txt.ClientID,
                                                 String.Format("document.getElementById('{0}').focus();", txt.ClientID) + string.Empty + String.Format("document.getElementById('{0}').value=document.getElementById('{0}').value;", txt.ClientID),
                                                 true);
            }
        }
       
        public static bool CheckForNull(this object obj)
        {

            if (obj == null)
            {

                return false;

            }

            else
            {

                return true;

            }

        }

        public static Int16 ToShort(this string obj)
        {
            Int16 value;
            if (!Int16.TryParse(obj, out value))
                value = 0;
            return value;
        }

        public static Int64 ToLong(this string obj)
        {
            Int64 value;
            if (!Int64.TryParse(obj, out value))
                value = 0;
            return value;

        }

        public static int ToInt(this string obj)
        {

            int value;
            if (!int.TryParse(obj, out value))
                value = 0;
            return value;

        }

        public static int? ToNullableInt(this string obj)
        {
            if (obj == string.Empty)
                return null;
            else
                return obj.ToInt();
        }



        public static string ToText(this object obj)
        {

            string value = string.Empty;
            if (obj !=null)
                value = obj.ToString();
            return value;

        }

        public static double ToDouble(this object obj)
        {

            double num = 0.0;

            double.TryParse(obj.ToString(), out num);

            return num;

        }

        public static decimal ToDecimal(this string obj)
        {
            decimal value;
            if (!decimal.TryParse(obj, out value))
                value = 0;
            return value;

        }


        public static bool ToBoolean(this string obj)
        {
            bool value;
            if (!bool.TryParse(obj, out value))
                value = false;
            return value;

        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
      
        public static string UppercaseFirstLetter(this string value)
        {
            //
            // Uppercase the first letter in the string this extension is called on.
            //
            if (value.Length > 0)
            {
                char[] array = value.ToCharArray();
                array[0] = char.ToUpper(array[0]);
                return new string(array);
            }
            return value;
        }
      
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
      
        public static bool IsLessThanOrEqualTo<T>(this T lValue, T value) where T : IComparable<T>, IEquatable<T>
        {
            return lValue.CompareTo(value) < 0 || lValue.Equals(value);
        }
       
        public static bool In<T>(this T o, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (item.Equals(o))
                    return true;
            }

            return false;
        }

    }
}
