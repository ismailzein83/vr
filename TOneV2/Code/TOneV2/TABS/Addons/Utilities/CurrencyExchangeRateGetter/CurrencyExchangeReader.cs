using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace TABS.Addons.Utilities.CurrencyExchangeRateGetter
{
    /// <summary>
    /// CurrencyConverter
    /// </summary>
    public class CurrencyReader
    {
        public CurrencyReader()
        {
        }

        // private variable to store a copy of returned raw HTML
        private string rawHtml;
        protected const string SymbolGroup = "SYMBOL";
        protected const string NameGroup = "NAME";
        protected const string RateGroup = "RATE2";
        public string ModuleError;
        public string ExchangeRatesURL;


        /// <summary>
        /// Build the URL 
        /// </summary>
        private string buildUrl()
        {
            string url = "";
            if (ExchangeRatesURL != null && ExchangeRatesURL != String.Empty)
            {
                url = ExchangeRatesURL;
            }
            else
            {
                ModuleError = "URL was Empty!!!";
                url = "";
            }
            return url;
        }

        /// <summary>
        /// Connect to web site by passing a URL and word
        /// Method to return the result in HTML 
        /// </summary>
        private String readHtmlPage(string url)
        {
            string result = "";
            try
            {
                WebRequest objRequest = HttpWebRequest.Create(url);
                WebResponse objResponse = objRequest.GetResponse();
                Stream stream = objResponse.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                result = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                ModuleError = e.Message;
            }

            return (result);
        }

        /// <summary>
        /// This Method will initialize the process
        /// </summary>
        private bool Connect()
        {
            bool result = false;
            string url = "";
            string returnHtml = "";

            try
            {
                url = buildUrl();
                if (url != String.Empty && url != "")
                {
                    returnHtml = readHtmlPage(url);

                    // add returnHtml to rawHtml
                    rawHtml = returnHtml;

                    result = true;
                }
            }
            catch (Exception e)
            {
                ModuleError = e.Message;
                result = false;
            }
            return result;
        }


        public DataTable Build()
        {

            bool status = false;

            status = Connect();
            if (status == true)
            {
                if (ExchangeRatesURL != null && ExchangeRatesURL != String.Empty)
                {
                   ModuleError = "Connected";
                   return BuildCurrencyExchangeTable();
                }
                else
                {
                    ModuleError = "URL path was not provided!!!";
                    return null;
                }
            }
            else
            {
                ModuleError = "Not Connected";
                return null;
            }

        }

        public DataTable BuildCurrencyExchangeTable()
        {
            try
            {
                DataTable DT = new DataTable();
                DT.Columns.Add("Symbol", typeof(String));
                DT.Columns.Add("Name", typeof(String));
                DT.Columns.Add("LastRate", typeof(float));

                foreach (Match matchingRow in regex.Matches(rawHtml))
                    Add(matchingRow, DT);
                
                return DT;
            }
            catch (Exception e)
            {
                ModuleError = e.Message;
                return null;
            }
        }

        static Regex regex = new Regex(@"<tr.*>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=center width=40><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;<b><tt>(?<SYMBOL>\w+)</tt></b>\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=left width=273><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;(?<NAME>(\w|\s)+)\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=right width=80><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;(?<RATE1>(\d+(.\d+)*))\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=right width=80><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;(?<RATE2>(\d+(.\d+)*))\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=right width=80><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;(?<RATE3>(\d+(.\d+)*))\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>\s+<td align=right width=80><font size=2 face=""New Times Roman,Times,Serif"">\&nbsp;(?<RATE4>(\d+(.\d+)*))\&nbsp;</font></td>\s+<td width=1 height=0 bgcolor=""#800000""><img src=""/img/spacer.gif"" width=1 height=0 alt=""\|""/></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        void Add(Match matchingRow, DataTable table)
        {
            DataRow row = table.NewRow();
            row["Symbol"] = matchingRow.Groups[SymbolGroup].Value;
            row["Name"] = matchingRow.Groups[NameGroup].Value;
            row["LastRate"] = float.Parse(matchingRow.Groups[RateGroup].Value);
            table.Rows.Add(row);
        }

    }

}
