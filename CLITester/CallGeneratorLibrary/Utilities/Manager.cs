using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CallGeneratorLibrary.Utilities
{
    public class Manager
    {
        public static bool IsValidEnum<T>(short val)
        {
            Type enumType = typeof(T);

            if (!Enum.IsDefined(enumType, val))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidEmail(string inputData)
        {
            // Return true if inputData is in valid e-mail format.
            return Regex.IsMatch(inputData, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        #region Date functions
        public string FormatToDate(string sformat, string sdate)
        {
            DateTime ddate;

            ddate = DateTime.Parse(sdate);

            return string.Format(sformat, ddate);
        }

        public String ConvertDate(String mydate)
        {
            if (mydate.Equals(""))
                return "";
            string[] ar;
            ar = (mydate).Split('/');
            return ((ar[1].Length == 1) ? "0" + ar[1] : ar[1]) + "/" + ((ar[0].Length == 1) ? "0" + ar[0] : ar[0]) + "/" + ar[2];
        }

        public Boolean ValidDate(string objDate)
        {
            Int32 nMonth;
            Int32 nDay;
            Int32 nYear;

            Boolean bValidDate;
            objDate = ConvertDate(objDate);
            nDay = Convert.ToInt32(objDate.Substring(0, 2));
            nMonth = Convert.ToInt32(objDate.Substring(3, 2));
            nYear = Convert.ToInt32(objDate.Substring(6, 4));
            if (nMonth == 1 || nMonth == 3 || nMonth == 5 || nMonth == 7 || nMonth == 8 || nMonth == 10 || nMonth == 12)
            {
                if (nDay <= 31 && nDay > 0) { bValidDate = true; }
                else { bValidDate = false; }
            }
            else
            {
                if (nMonth == 4 || nMonth == 6 || nMonth == 9 || nMonth == 11)
                {
                    if (nDay <= 30 && nDay > 0) { bValidDate = true; }
                    else { bValidDate = false; }
                }
                else
                {
                    if ((nYear % 4) == 0)
                    {
                        if (nDay <= 29 && nDay > 0) { bValidDate = true; }
                        else { bValidDate = false; }
                    }
                    else
                    {
                        if (nDay <= 28 && nDay > 0) { bValidDate = true; }
                        else { bValidDate = false; }
                    }
                }
            }
            return bValidDate;
        }

        public Boolean IsDateValid(string objDate)
        {
            Int16 nMonth;
            Int16 nDay;
            Int16 nYear;
            string[] ar;

            ar = (objDate).Split('/');

            nDay = Convert.ToInt16((ar[0].Length == 1) ? "0" + ar[0] : ar[0]);
            nMonth = Convert.ToInt16((ar[1].Length == 1) ? "0" + ar[1] : ar[1]);
            nYear = Convert.ToInt16(ar[2]);

            System.Globalization.CultureInfo info =
                new System.Globalization.CultureInfo("en-US", false);

            System.Globalization.Calendar calendar = info.Calendar;
            try
            {
                System.DateTime dateTime1 =
                    new System.DateTime(nYear, nMonth, nDay,
                    0, 0, 0, 0, calendar
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Text input Checking

        public static string ToTitleCase(string inputData, bool forceCasing)
        {
            if (forceCasing)
            {
                inputData = inputData.ToLower();
            }

            return (new CultureInfo("en-US").TextInfo.ToTitleCase(inputData));
        }

        public static bool IsValidIPAddress(string inputData)
        {
            return Regex.IsMatch(inputData, @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$");
        }

        /// <summary>
        /// Cleans the input string from nonalphanumeric characters
        /// </summary>
        /// <param name="input">String to clean</param>
        /// <param name="exceptCharacters">string of nonalphanumeric characters that must be not removed, ex: ".@-"</param>
        /// <returns></returns>
        public static string CleanInput(string input, string exceptCharacters)
        {
            if (exceptCharacters != string.Empty)
                exceptCharacters = @"\" + exceptCharacters;

            return Regex.Replace(input, @"[^\w" + exceptCharacters + "]", string.Empty);
        }

        public bool IsRepeated(string sText)
        {
            int code; int ncount = 1;
            Encoding ascii = Encoding.ASCII;
            byte[] b = ascii.GetBytes(sText);

            code = b[0];
            for (int i = 1; i <= b.GetUpperBound(0); i++)
            {
                if (code == b[i])
                    ncount = ncount + 1;
                else
                {
                    code = b[i];
                    ncount = 1;
                }
                if (ncount == 3)
                    return true;
            }
            return false;
        }

        public bool IsAlphaNumeric(string sText)
        {
            Boolean bAlphaNum = false;
            string sChar;
            string sAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string sNum = "0123456789";

            sText = sText.ToUpper(); //Convert sText To uppercase to make comparisons easier.

            for (int i = 0; i < sText.Length && bAlphaNum == false; i++)
            {
                sChar = sText.Substring(i, 1);
                if (sAlpha.IndexOf(sChar) != -1)
                    bAlphaNum = true;

            }
            if (bAlphaNum == false)
                return bAlphaNum;

            bAlphaNum = false;

            for (int i = 0; i < sText.Length && bAlphaNum == false; i++)
            {
                sChar = sText.Substring(i, 1);
                if (sNum.IndexOf(sChar) != -1)
                    bAlphaNum = true;
            }

            return bAlphaNum;
        }

        public bool IsSorted(string sText, string sCompareVar)
        {
            bool bSorted = false;
            string sChar;

            for (int i = 0; i < sText.Length && bSorted == false; i++)
            {
                if (sText.Substring(i).Length >= 3)
                {
                    sChar = sText.Substring(i, 3);
                    if (sCompareVar.ToLower().IndexOf(sChar.ToLower()) != -1)
                        bSorted = true;
                }
            }
            return bSorted;
        }

        public static bool IsInt32(string inputData)
        {
            try
            {
                Int32.Parse(inputData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInt64(string inputData)
        {
            try
            {
                Int64.Parse(inputData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDouble(string inputData)
        {
            try
            {
                double.Parse(inputData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsWhiteSpace(string inputdata)
        {
            string whiteSpace = " \t\n\r";
            string sChar;

            for (int i = 0; i < inputdata.Trim().Length; i++)
            {
                sChar = inputdata.Substring(i, 1);

                if (whiteSpace.IndexOf(sChar) != -1)
                    return true;
            }

            return false;
        }

        public static bool IsControlChr(string inputData)
        {
            string CtrlChr = "()/|?,;:'~<>\\+=.[]{}";
            string sChar;

            for (int i = 0; i < inputData.Trim().Length; i++)
            {
                sChar = inputData.Substring(i, 1);

                if (CtrlChr.IndexOf(sChar) != -1)
                    return true;
            }

            return false;
        }

        #endregion

        public static string GetHash(string sinputdata)
        {
            MD5 md = MD5CryptoServiceProvider.Create();
            byte[] hash;
            StringBuilder sb = new StringBuilder();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] buffer = enc.GetBytes(sinputdata);
            hash = md.ComputeHash(buffer);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetUnHash(string data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();// Encoding.UTF8.GetString(result);
        }

        public static string GetRandom()
        {
            return new Random().Next(10, 999999).ToString();
        }
    }
}
