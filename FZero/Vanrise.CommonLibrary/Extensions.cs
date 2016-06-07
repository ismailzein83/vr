
namespace Vanrise.CommonLibrary
{
    public static class ExtensionMethods
    {

        public static int ToInt(this string obj)
        {

            int value;
            if (!int.TryParse(obj, out value))
                value = 0;
            return value;

        }

        public static string ToText(this object obj)
        {

            string value = string.Empty;
            if (obj != null)
                value = obj.ToString();
            return value;

        }

        public static bool ToBoolean(this string obj)
        {
            bool value;
            if (!bool.TryParse(obj, out value))
                value = false;
            return value;

        }
    }
}
