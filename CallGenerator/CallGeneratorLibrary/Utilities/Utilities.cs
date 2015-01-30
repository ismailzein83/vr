using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace CallGeneratorLibrary.Utilities
{
    public class Utilities
    {
        public static int SafeIntParse(string value)
        {
            return SafeIntParse(value, 0);
        }
        public static int GetNumberOfDigits(string dataString)
        {
            int count = 0;
            for (int i = 0; i < dataString.Length; i++)
            {
                if (Char.IsDigit(dataString[i]))
                {
                    count++;
                }
            }
            return count;
        }
        public static int SafeIntParse(string value, int nullValue)
        {
            double result = 0;

            if (Double.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result) == true)
            {
                if (result > int.MaxValue || result < int.MinValue)
                {
                    return nullValue;
                }
                else
                {
                    return (int)result;
                }
            }
            else
            {
                return nullValue;
            }
        }

        #region Html Text
        public static string GetTextFromHTML(string HTML)
        {
            return GetTextFromHTML(HTML, false);
        }
        public static string GetSubText(string text, int charsNumber)
        {
            if (text == "")
                return "";

            string result = "";
            try
            {
                result = text.Substring(0, (text.Length <= charsNumber ? text.Length : charsNumber));
                result = result.Replace("\n", "").Replace("\r", "");
                result = result.Trim();

                int index = result.Trim().LastIndexOf(" ");

                if (index > 0)
                    result = result.Remove(index, result.Length - index) + "...";
            }
            catch //(Exception ex)
            {

            }
            return result;
        }
        public static string GetTextFromHTML(string HTML, bool keepBR)
        {
            if (HTML == null || HTML.Length < 1) return "";

            if (keepBR)
            {
                HTML = HTML.Replace("<BR>", "$BR$");
                HTML = HTML.Replace("<BR/>", "$BR$");
                HTML = HTML.Replace("<BR />", "$BR$");
                HTML = HTML.Replace("<br>", "$BR$");
                HTML = HTML.Replace("<br/>", "$BR$");
                HTML = HTML.Replace("<br />", "$BR$");
                HTML = HTML.Replace("</p>", "$BR$");
                HTML = HTML.Replace("</P>", "$BR$");
            }

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

            if (keepBR)
            {
                buffer = buffer.Replace("$BR$", "<BR>");
            }
            return buffer.ToString();
        }
        #endregion

        #region DateTime Utilities
        public static string GetStringFromDateTime(DateTime date)
        {
            return date.Day.ToString("00") + date.Month.ToString("00") + date.Year.ToString("0000");
        }
        public static DateTime GetDateTimeFromQueryString(string s)
        {
            //input: a string represent a DateTime in the form: mmddyyyy
            //output: the DateTime representation of the input string
            DateTime retDate = DateTime.MinValue;

            if ((s != null) && s.Trim() != "")
            {
                try
                {
                    int month = int.Parse(s.Substring(2, 2)), day = int.Parse(s.Substring(0, 2)), year = int.Parse(s.Substring(4));
                    retDate = new DateTime(year, month, day);
                }
                catch { }
            }
            return retDate;
        }
        #endregion

        #region Mail Utilities
        public static bool IsEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        public static bool IsValidEmail(string inputData)
        {
            // Return true if inputData is in valid e-mail format.
            return Regex.IsMatch(inputData, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        public static void SendEmail(string subject, string body, string from, string to)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            message.To.Add(to);

            if (string.IsNullOrEmpty(from))
            {
                message.From = new System.Net.Mail.MailAddress(Config.SendingEmail);
            }
            else
            {
                message.From = new System.Net.Mail.MailAddress(from);
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = Config.SmtpServer;

            try
            {
                smtp.Send(message);
            }
            catch
            {

            }
        }
        #endregion

        #region Serialize Object
        /// <summary>
        /// Serializes a LINQ object to an XML string
        /// </summary>
        /// <typeparam name="T">Type of the Object</typeparam>
        /// <param name="linqObject">The LINQ object to convert</param>
        /// <returns>string</returns>
        public static string SerializeLINQtoXML<T>(T linqObject)
        {
            DataContractSerializer dcs = new DataContractSerializer(linqObject.GetType());
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);

            try
            {
                dcs.WriteObject(writer, linqObject);
                writer.Close();
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Deserializes an XML string to a LINQ object
        /// </summary>
        /// <typeparam name="T">The type of the LINQ Object</typeparam>
        /// <param name="input">XML input</param>
        /// <returns>Type of the LINQ Object</returns>
        public static T DeserializeLINQfromXML<T>(string input)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            TextReader treader = new StringReader(input);
            XmlReader reader = XmlReader.Create(treader);
            T linqObject = (T)dcs.ReadObject(reader, true);
            reader.Close();

            return linqObject;
        }
        #endregion

        public static string HighlightWordsInText(string wordsToHighlight, string Text)
        {
            //highlights wordsToHighlight in Text and return Text (hightlighted)
            string RetValue = string.Empty;

            if (wordsToHighlight != "")
            {
                char[] sep = new char[] { ' ' };
                string[] words = wordsToHighlight.Split(sep);

                for (int j = 0; j <= words.Length - 1; j++)
                {
                    Text = InsensitiveReplace(Text, words[j], "<font color='red'>" + words[j] + "</font>");
                }
                RetValue = Text;
            }
            return RetValue;
        }
        public static string InsensitiveReplace(string textToSearch, string oldValue, string newValue)
        {
            // textToSearch: the whole text in which the Replace will happen
            // oldValue: the substring within textToSearch that is to be Replaced (insensitively)
            // newValue: the replaced value of oldValue
            string strReturn, tempHold;
            tempHold = GetCaseInsensitiveSearch(oldValue);
            strReturn = Regex.Replace(textToSearch, tempHold, newValue);
            return strReturn;
        }
        // Creates a case-insensitive regular expression search string
        // For Example:
        //	"[fF][oO][oO][bB][aA][rR]"= GetCaseInsensitiveSearch("FooBar");
        public static string GetCaseInsensitiveSearch(String strSearch)
        {
            String strReturn = "";
            char chrCurrent;
            char chrLower;
            char chrUpper;
            int intCounter;

            for (intCounter = 0; intCounter < strSearch.Length; intCounter++)
            {
                chrCurrent = strSearch[intCounter];
                chrLower = char.ToLower(chrCurrent);
                chrUpper = char.ToUpper(chrCurrent);
                if (chrUpper == chrLower)
                    strReturn = strReturn + chrCurrent;
                else
                    strReturn = strReturn + "[" + chrLower + chrUpper + "]";
            }
            return strReturn;
        }
    }
}
