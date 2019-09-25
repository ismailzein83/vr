using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Common
{
    public static class Utilities
    {
        static string[] harakat = new string[] {
                                                   ((char)1614).ToString(), ((char)1611).ToString(),
                                                   ((char)1615).ToString(), ((char)1612).ToString(),
                                                   ((char)1616).ToString(), ((char)1613).ToString(),
                                                   ((char)1618).ToString(), ((char)1617).ToString()
                                               };

        static char[] characters = new char[] {
                                                  '!', '\\','"','#', '$', '%','&',
                                                  '\'', '(', ')', '*', '+', ',',
                                                  '-', '.', '/', ':', '?', '[',
                                                  ']', '_', '{', '|', '}', '~',
                                                  '،', (char) 168, 'ـ',
                                                  '؟'
                                              };
        public static Q GetEnumAttribute<T, Q>(T enumItem)
            where T : struct
            where Q : Attribute
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(String.Format("{0} is not an Enum type", enumType));

            Type attributeType = typeof(Q);
            Dictionary<T, Q> enumAttributes = new Dictionary<T, Q>();
            foreach (var member in enumType.GetFields())
            {
                T memberAsEnum;
                if (Enum.TryParse<T>(member.Name, true, out memberAsEnum) && memberAsEnum.Equals(enumItem))
                {
                    return member.GetCustomAttributes(attributeType, true).FirstOrDefault() as Q;
                }

            }
            return default(Q);
        }

        public static string TrimPrefixOnly(string sourceText)
        {
            return TrimAL(sourceText, true);
        }

        public static string TrimAL(string sourceText)
        {
            return TrimAL(sourceText, false);
        }

        private static string TrimAL(string word, bool onlyPrefix)
        {
            //trim the word
            string newWord = word.Trim();

            //remove other unnecessary characters
            #region Unnecessary Characeters

            for (int i = 0; i < characters.Length; i++)
                newWord = newWord.Replace(characters[i], ' ');

            #endregion Unnecessary Characeters

            #region Harakat
            for (int i = 0; i < harakat.Length; i++)
                newWord = newWord.Replace(harakat[i], "");
            #endregion Harakat

            //remove every double occurrence of whitspace
            //System.Text.RegularExpressions.Regex cleaner = new System.Text.RegularExpressions.Regex("\\s+");
            //newWord = cleaner.Replace(newWord, " ");

            //remove all spaces
            System.Text.RegularExpressions.Regex cleaner = new System.Text.RegularExpressions.Regex(@"([^\s])(\s)");
            newWord = cleaner.Replace(newWord, "$1");

            newWord = " " + newWord + " ";

            /*  ال  al  */
            string alWord = " " + "ال";

            newWord = newWord.Replace(alWord, " ");

            /*  لل  lil  */
            string lilWord = " " + "لل";
            newWord = newWord.Replace(lilWord, " ");

            string prefix = "";
            if (newWord.Trim().Length > 4)
            {
                prefix = " " + "فال"; //   فال fal  
                newWord = newWord.Replace(prefix, " ");

                prefix = " " + "بال"; /*   بال  bal */
                newWord = newWord.Replace(prefix, " ");

                prefix = " " + "كال"; /*   كال  kal  */
                newWord = newWord.Replace(prefix, " ");

                prefix = " " + "وال";/*   وال  wal  */
                newWord = newWord.Replace(prefix, " ");
            }

            newWord = newWord.Replace((char)188, ' ');
            newWord = newWord.Replace((char)249, ' ');

            if (!onlyPrefix)
            {
                string correctChar = "ا";
                /* أ alef hamza*/
                newWord = newWord.Replace("أ", correctChar);

                /*  إ alef hamza kasra*/
                newWord = newWord.Replace("إ", correctChar);

                /*  آ  alef madda*/
                newWord = newWord.Replace("آ", correctChar);

                /* ة ta marbouta*/
                newWord = newWord.Replace("ة" + " ", "ه" + " ");

                /*  يء  ya2 hamzah  */
                newWord = newWord.Replace("ي" + "ء" + " ", "ى" + " ");

                /*  وء  wa hamzah  */
                newWord = newWord.Replace("و" + "ء", "و");

                /*  ؤ  ou  */
                newWord = newWord.Replace("ؤ", "و");

                /* ى alef maasoura*/
                newWord = newWord.Replace("ى" + " ", "ا" + " ");
            }

            System.Text.RegularExpressions.Regex cleaner2 = new System.Text.RegularExpressions.Regex(@"([^\s])(\s)");
            newWord = cleaner2.Replace(newWord, "$1");

            //newWord = cleaner.Replace(newWord, " ");

            return newWord.Trim();
        }
    }
}
