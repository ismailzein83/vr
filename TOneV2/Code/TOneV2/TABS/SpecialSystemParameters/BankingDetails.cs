using System;
using System.Collections.Generic;
using System.Text;

namespace TABS.SpecialSystemParameters
{
    public class BankingDetails : BaseXmlDetails
    {
        public string GUID { get { return Get("GUID"); } set { Set("GUID", value); } }
        public string Bank { get { return Get("Bank"); } set { Set("Bank", value); } }
        public string Address { get { return Get("Address"); } set { Set("Address", value); } }
        public string AccountNumber { get { return Get("AccountNumber"); } set { Set("AccountNumber", value); } }
        public string CurrencyID { get { return Get("CurrencyID"); } set { Set("CurrencyID", value); } }
        public string AccountCode { get { return Get("AccountCode"); } set { Set("AccountCode", value); } }
        public string SwiftCode { get { return Get("SwiftCode"); } set { Set("SwiftCode", value); } }
        public string AccountHolder { get { return Get("AccountHolder"); } set { Set("AccountHolder", value); } }
        public string SortCode { get { return Get("SortCode"); } set { Set("SortCode", value); } }
        public string IBAN { get { return Get("IBAN"); } set { Set("IBAN", value); } }
        public string CorrespondentBank { get { return Get("CorrespondentBank"); } set { Set("CorrespondentBank", value); } }
        public string CorrespondentBankSwiftCode { get { return Get("CorrespondentBankSwiftCode"); } set { Set("CorrespondentBankSwiftCode", value); } }

        /// <summary>
        /// Returns the list of available banking details from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<BankingDetails> Get(SystemParameter parameter) { return BaseXmlDetails.Get<BankingDetails>(parameter); }

        /// <summary>
        /// Create a new Banking Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static BankingDetails Create(SystemParameter parameter, out List<BankingDetails> detailsList) { return BaseXmlDetails.Create<BankingDetails>(parameter, out detailsList, SystemParameter.DefaultXml); }

        /// <summary>
        /// Save the System Parameter banking Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        public static Exception Save(SystemParameter parameter, List<BankingDetails> details) { return BaseXmlDetails.Save(parameter, details, SystemParameter.DefaultXml); }

        /// <summary>
        /// Remove the banking details at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(SystemParameter parameter, int index) { BaseXmlDetails.Remove<BankingDetails>(parameter, index, SystemParameter.DefaultXml); }

        public string DefinitionDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(Bank)) sb.AppendFormat("Bank: {0}\r\n", Bank);
                if (!string.IsNullOrEmpty(Address)) sb.AppendFormat("Address: {0}\r\n", Address);
                if (!string.IsNullOrEmpty(AccountHolder)) sb.AppendFormat("Account Holder: {0}\r\n", AccountHolder);
                if (!string.IsNullOrEmpty(CurrencyID)) sb.AppendFormat("Currency: {0}\r\n", Currency.Visible[CurrencyID].Name);
                if (!string.IsNullOrEmpty(SortCode)) sb.AppendFormat("Sort Code: {0}\r\n", SortCode);
                if (!string.IsNullOrEmpty(AccountNumber)) sb.AppendFormat("Account Nbr: {0}\r\n", AccountNumber);
                if (!string.IsNullOrEmpty(AccountCode)) sb.AppendFormat("Account Code: {0}\r\n", AccountCode);
                if (!string.IsNullOrEmpty(IBAN)) sb.AppendFormat("IBAN: {0}\r\n", IBAN);
                if (!string.IsNullOrEmpty(SwiftCode)) sb.AppendFormat("Swift Code: {0}\r\n", SwiftCode);
                if (!string.IsNullOrEmpty(CorrespondentBank)) sb.AppendFormat("Correspondent Bank: {0}\r\n", CorrespondentBank);
                if (!string.IsNullOrEmpty(CorrespondentBankSwiftCode)) sb.AppendFormat("Correspondent Bank Swift Code: {0}\r\n", CorrespondentBankSwiftCode);

                return sb.ToString();
            }
        }

        public string TextToDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(Bank)) sb.AppendFormat("{0} ", Bank);
                if (!string.IsNullOrEmpty(CurrencyID)) sb.AppendFormat(",{0} ", Currency.Visible[CurrencyID].Name);
                if (!string.IsNullOrEmpty(AccountNumber)) sb.AppendFormat(",{0} ", AccountNumber);

                return sb.ToString();
            }
        }
    }
}