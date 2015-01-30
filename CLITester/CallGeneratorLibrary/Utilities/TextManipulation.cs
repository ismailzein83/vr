using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace CallGeneratorLibrary.Utilities
{
    public class TextManipulation
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

        public static string CleanFromHarakat(string sourceText)
        {
            for (int i = 0; i < characters.Length; i++)
                sourceText = sourceText.Replace(characters[i], ' ');

            for (int i = 0; i < harakat.Length; i++)
                sourceText = sourceText.Replace(harakat[i], "");

            return sourceText;
        }

        /// <summary>
        /// Returns 'pure' Text from HTML. Eliminates unnecessary white space and tags.
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            return GetTextFromHTML(HTML, true);
        }

        /// <summary>
        /// Returns 'pure' Text from HTML. Eliminates unnecessary white space and tags.
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML, bool preserveBr)
        {
            if (HTML == null || HTML.Length < 1) return "";

            ////stlyes
            HTML = Regex.Replace(HTML, @"<[sS][tT][yY][lL][eE]>[\s\S]*?</[sS][tT][yY][lL][eE]>", string.Empty);
            //meta tags
            HTML = Regex.Replace(HTML, @"<[mM][eE][tT][aA][\s\S]*?>", string.Empty);
            //link tags
            HTML = Regex.Replace(HTML, @"<[lL][iI][nN][kK][\s\S]*?>", string.Empty);
            ////font tags
            //HTML = Regex.Replace(HTML, @"<[fF][oO][nN][tT] [\s\S]*?>[\s\S]*?</[fF][oO][nN][tT]>", string.Empty);
            //xml tags
            HTML = Regex.Replace(HTML, @"<[xX][mM][lL]>[\s\S]*?</[xX][mM][lL]>", string.Empty);

            if (preserveBr)
            {
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[bB][rR](.*?)>", "$BR$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[pP][A-Za-z0-9 \"\'=;-]*>", "$SP$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "</[pP]>", "$EP$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[sS][tT][rR][oO][nN][gG]>", "$SS$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "</[sS][tT][rR][oO][nN][gG]>", "$ES$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[bB]>", "$SS$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "</[bB]>", "$ES$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[uU]>", "$SU$");
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "</[uU]>", "$EU$");
            }
            else
                HTML = System.Text.RegularExpressions.Regex.Replace(HTML, "<[bB][rR](.*?)>", "$BR$");

            StringBuilder buffer = new StringBuilder(HTML.Length);

            int index = -1, limit = HTML.Length;
            char currentChar = ' ', previousChar = ' ';
            bool insideTagDef = false;
            while (++index < limit)
            {
                currentChar = HTML[index];

                if ((int)currentChar < 32) currentChar = ' ';
                if (currentChar == ' ' && previousChar == ' ') continue;

                // Inside a Tag?
                if (currentChar == '<' && index < limit - 1)
                {
                    char nextChar = HTML[index + 1];
                    if ((int)nextChar < 32) // white space? then this is not a tag start
                    {
                        previousChar = currentChar;
                        buffer.Append(currentChar);
                    }
                    else // it must be a tag start
                    {
                        insideTagDef = true;
                    }
                }
                // Closing a tag
                else if (currentChar == '>')
                {
                    if (insideTagDef)
                    {
                        insideTagDef = false;
                    }
                    else
                    {
                        previousChar = currentChar;
                        buffer.Append(currentChar);
                    }
                }
                // Normal Character
                else if (!insideTagDef)
                {
                    previousChar = currentChar;
                    buffer.Append(currentChar);
                }
            }

            //HTML = Regex.Replace(HTML, "<[^>]*>", string.Empty);
            //HTML.Replace("&nbsp;", " ");
            if (preserveBr)
            {
                buffer = buffer.Replace("$BR$", "<br/>");
                buffer = buffer.Replace("$SP$", "<p>");
                buffer = buffer.Replace("$EP$", "</p>");
                buffer = buffer.Replace("$SS$", "<strong>");
                buffer = buffer.Replace("$ES$", "</strong>");
                buffer = buffer.Replace("$SU$", "<u>");
                buffer = buffer.Replace("$EU$", "</u>");
            }
            else
                buffer = buffer.Replace("$BR$", " ");
            return buffer.ToString();
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
            System.Text.RegularExpressions.Regex cleaner = new System.Text.RegularExpressions.Regex("\\s+");
            newWord = cleaner.Replace(newWord, " ");

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
                newWord = newWord.Replace("ه" + " ", "ة" + " ");

                /*  يء  ya2 hamzah  */
                newWord = newWord.Replace("ي" + "ء" + " ", "ى" + " ");

                /*  وء  wa hamzah  */
                newWord = newWord.Replace("و" + "ء", "و");

                /*  ؤ  ou  */
                newWord = newWord.Replace("ؤ", "و");
            }

            newWord = cleaner.Replace(newWord, " ");

            return newWord.Trim();
        }

        public static string GetSubText(string text, int charsNumber)
        {
            string result;
            text = GetTextFromHTML(text, true);

            string newText = text.Replace("<br/>", "$$").Replace("<p>", "$$").Replace("</p>", "$$");
            string[] list = newText.Split('$');
            text = "";
            foreach (string name in list)
            {
                if (name != "" && name != " ")
                    text += name + "<br/>";
            }

            result = text.Substring(0, (text.Length <= charsNumber ? text.Length : charsNumber));

            int index = result.Trim().LastIndexOf(' ');

            if (index > 0)
                result = result.Remove(index, result.Length - index);

            return result + "..";
        }

        public static string HighlightWords(string words, string contents)
        {
            char[] separators = { ' ' };
            string[] searchWords = words.Split(separators);
            string[] contentsWord = contents.Split(separators);

            for (int i = 0; i <= searchWords.Length - 1; i++)
            {
                if (searchWords[i].Trim() != "")
                {
                    for (int j = 0; j < contentsWord.Length; j++)
                    {
                        if (
                            TextManipulation.TrimAL(contentsWord[j]) == TextManipulation.TrimAL(searchWords[i])
                            ||
                            searchWords[i] == contentsWord[j]
                            )
                            contents = contents.Replace(contentsWord[j], "<span class='searchHighlight'>" + contentsWord[j] + "</span>");
                    }
                }
            }

            return contents;
        }

        public static string CleanWordHtml(string html)
        {
            StringCollection sc = new StringCollection();

            // Get rid of unnecessary tags
            sc.Add(@"<(meta|link|/?o:|/?style|/?div|/?st\d|/?head|/?html|body|/?body|/?span|!\[)[^>]*?>");
            // get rid of unnecessary tag spans (comments and title)
            sc.Add(@"<!--(\w|\W)+?-->");
            sc.Add(@"<title>(\w|\W)+?</title>");
            // Get rid of classes and styles
            sc.Add(@"\s?class=\w+");
            sc.Add(@"\s+style='[^']+'");
            // Get rid of empty paragraph tags
            sc.Add(@"(<[^>]+>)+&nbsp;(</\w+>)+");
            // remove bizarre v: element attached to <img> tag
            sc.Add(@"\s+v:\w+=""[^""]+""");
            // remove extra lines
            sc.Add(@"(\n\r){2,}");
            foreach (string s in sc)
            {
                html = Regex.Replace(html, s, string.Empty, RegexOptions.IgnoreCase);
            }
            return html;
        }

        public static string CleanHtml(string html)
        {
            // start by completely removing all unwanted tags     
            html = Regex.Replace(html, @"<[/]?(font|span|xml|del|ins|[ovwxp]:\w+)[^>]*?>", "", RegexOptions.IgnoreCase);
            // then run another pass over the html (twice), removing unwanted attributes     
            html = Regex.Replace(html, @"<([^>]*)(?:class|lang|style|size|face|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<([^>]*)(?:class|lang|style|size|face|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            return html;
        }
    }
}
