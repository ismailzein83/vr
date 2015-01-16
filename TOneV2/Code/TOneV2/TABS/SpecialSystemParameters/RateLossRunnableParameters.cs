using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS.SpecialSystemParameters
{
    public class RateLossRunnableParameters : BaseXmlDetails
    {
        public string Date { get { return Get("Date"); } set { Set("Date", value); } }
        public string ShowFrom { get { return Get("ShowFrom"); } set { Set("ShowFrom", value); } }
        public string MailFrom { get { return Get("MailFrom"); } set { Set("MailFrom", value); } }
        public string MailTo { get { return Get("MailTo"); } set { Set("MailTo", value); } }
        public string IsAmountVisible { get { return Get("IsAmountVisible"); } set { Set("IsAmountVisible", value); } }
        public string IsDurationVisible { get { return Get("IsDurationVisible"); } set { Set("IsDurationVisible", value); } }
        public string Margin { get { return Get("Margin"); } set { Set("Margin", value); } }
        public string IncludedCustomers { get { return Get("IncludedCustomers"); } set { Set("IncludedCustomers", value); } }
        public string IncludedSuppliers { get { return Get("IncludedSuppliers"); } set { Set("IncludedSuppliers", value); } }
        public string IncludedSaleZones { get { return Get("IncludedSaleZones"); } set { Set("IncludedSaleZones", value); } }

        /// <summary>
        /// Returns the available rate loss runnable parameters from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<RateLossRunnableParameters> Get(SystemParameter parameter) { return BaseXmlDetails.Get<RateLossRunnableParameters>(parameter); }

        /// <summary>
        /// Create a new Rate Loss Runnable Parameters
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static RateLossRunnableParameters Create(SystemParameter parameter, out List<RateLossRunnableParameters> detailsList) { return BaseXmlDetails.Create<RateLossRunnableParameters>(parameter, out detailsList, SystemParameter.DefaultXml); }

        /// <summary>
        /// Save the System Parameter Rate Loss Runnable Parameters
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        public static Exception Save(SystemParameter parameter, List<RateLossRunnableParameters> details) { return BaseXmlDetails.Save(parameter, details, SystemParameter.DefaultXml); }

        /// <summary>
        /// Remove the rate loss runnable parameters
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(SystemParameter parameter, int index) { BaseXmlDetails.Remove<RateLossRunnableParameters>(parameter, index, SystemParameter.DefaultXml); }

        public static void SetDefaultSystemParameters(RateLossRunnableParameters parameters)
        {
            parameters.ShowFrom = "0";
            parameters.IsAmountVisible = "true";
            parameters.IsDurationVisible = "true";
            parameters.MailFrom = "support@vanrise.com";
            parameters.MailTo = "support@vanrise.com";
            parameters.Margin = "0";
            parameters.IncludedCustomers = TABS.CarrierAccount.Customers.Select(c => "'" + c.CarrierAccountID + "'").Aggregate((a, b) => a + "," + b);
            parameters.IncludedSuppliers = TABS.CarrierAccount.Suppliers.Select(s => "'" + s.CarrierAccountID + "'").Aggregate((a, b) => a + "," + b);
            parameters.IncludedSaleZones = TABS.Zone.OwnZones.Values.Select(z => z.ZoneID.ToString()).Aggregate((a, b) => a + "," + b);
        }

        public static string DefaultXml
        {
            get
            {
                return string.Format(@"
                            <?xml version='1.0' encoding='utf-8'?>
                                <SystemParameter>
                                    <RateLossRunnableParameters>
                                        <ShowFrom><![CDATA[{0}]]></ShowFrom>
                                        <IsAmountVisible><![CDATA[{1}]]></IsAmountVisible>
                                        <IsDurationVisible><![CDATA[{2}]]></IsDurationVisible>
                                        <MailFrom><![CDATA[{3}]]></MailFrom>
                                        <MailTo><![CDATA[{4}]]></MailTo>
                                        <Margin><![CDATA[{5}]]></Margin>
                                        <IncludedCustomers><![CDATA[{6}]]></IncludedCustomers>
                                        <IncludedSuppliers><![CDATA[{7}]]></IncludedSuppliers>
                                        <IncludedSaleZones><![CDATA[{8}]]></IncludedSaleZones>
                                    </RateLossRunnableParameters>
                                </SystemParameter>", "1"
                                                   , "true"
                                                   , "true"
                                                   , "support@vanrise.com"
                                                   , "support@vanrise.com"
                                                   , "0"
                                                   , TABS.CarrierAccount.Customers.Select(c => "'" + c.CarrierAccountID + "'").Aggregate((a, b) => a + "," + b)
                                                   , TABS.CarrierAccount.Suppliers.Select(s => "'" + s.CarrierAccountID + "'").Aggregate((a, b) => a + "," + b)
                                                   , TABS.Zone.OwnZones.Values.Select(z => z.ZoneID.ToString()).Aggregate((a, b) => a + "," + b)).Trim();
            }
        }
    }
}
