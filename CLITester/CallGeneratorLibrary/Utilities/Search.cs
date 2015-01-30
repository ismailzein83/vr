using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary.Utilities
{
    public class Search
    {
        public static string ConstructSearchText(string Text, bool andOperator, bool isExact)
        {
            string searchText = "";
            string[] words;
            char[] sep = { ' ' };
            words = Text.Split(sep);

            System.Collections.Specialized.StringCollection contents = new System.Collections.Specialized.StringCollection();
            contents.AddRange(words);

            for (int i = 0; i < contents.Count; i++)
                contents[i] = contents[i].Trim();

            for (int i = contents.Count - 1; i >= 0; i--)
                if (contents[i].Trim() == "")
                    contents.RemoveAt(i);

            if (!isExact)
            {
                string op = andOperator ? "AND" : "OR";

                for (int i = 0; i < contents.Count; i++)
                {
                    if (contents[i].Trim() != "")
                    {
                        string newWord = "";
                        string trimmedWord = TextManipulation.TrimAL(contents[i]);

                        newWord = "(";
                        newWord += "\"" + trimmedWord + "*\"" + " OR " + "\"" + contents[i] + "*\"";
                        newWord += ")";
                        contents[i] = newWord;
                    }
                }

                for (int i = 0; i < contents.Count - 1; i++)
                    searchText += contents[i] + " " + op + " ";

                searchText += contents[contents.Count - 1].Trim();
            }
            else
                searchText = "\"" + TextManipulation.TrimAL(Text) + "\" OR \"" + Text + "\"";

            return searchText;
        }
    }
}
