using System;
using System.Collections.Generic;

namespace TABS.SpecialSystemParameters
{
    public class CDRStoreDetails : BaseXmlDetails
    {
        public string ClassName { get { return Get("ClassName"); } set { Set("ClassName", value); } }
        public string Description { get { return Get("Description"); } set { Set("Description", value); } }
        public string ConfigString { get { return Get("ConfigString"); } set { Set("ConfigString", value); } }
        public string ConfigOptions { get { return Get("ConfigOptions"); } set { Set("ConfigOptions", value); } }
        public bool IsEnabled { get { return "Y".Equals(Get("IsEnabled")); } set { Set("IsEnabled", value ? "Y" : "N"); } }

        /// <summary>
        /// Returns the list of available banking details from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<CDRStoreDetails> Get(SystemParameter parameter) { return BaseXmlDetails.Get<CDRStoreDetails>(parameter); }

        /// <summary>
        /// Create a new Banking Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static CDRStoreDetails Create(SystemParameter parameter, out List<CDRStoreDetails> detailsList) { return BaseXmlDetails.Create<CDRStoreDetails>(parameter, out detailsList, SystemParameter.DefaultXml); }

        /// <summary>
        /// Save the System Parameter CDR Store Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        public static Exception Save(SystemParameter parameter, List<CDRStoreDetails> details) { return BaseXmlDetails.Save(parameter, details, SystemParameter.DefaultXml); }

        /// <summary>
        /// Remove the banking details at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(SystemParameter parameter, int index) { BaseXmlDetails.Remove<CDRStoreDetails>(parameter, index, SystemParameter.DefaultXml); }

        /// <summary>
        /// The Default XML for configured System Stores
        /// </summary>
        public static readonly string DefaultXml =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
                <SystemParameter>
                        <CDRStoreDetails>
                                <ClassName><![CDATA[Default CDR Store]]></ClassName>
                                <IsEnabled>Y</IsEnabled>
                        </CDRStoreDetails>
                </SystemParameter>";

    }
}
