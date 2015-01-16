using System;
using System.Collections.Generic;

namespace TABS.SpecialSystemParameters
{
    public class MailExpression : BaseXmlDetails
    {
        public string Value { get { return Get("Value"); } set { Set("Value", value); } }
        public string Mapping { get { return Get("Mapping"); } set { Set("Mapping", value); } }
        public string Description { get { return Get("Description"); } set { Set("Description", value); } }

        /// <summary>
        /// Returns the list of available banking details from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<MailExpression> Get(SystemParameter parameter) { return BaseXmlDetails.Get<MailExpression>(parameter); }

        /// <summary>
        /// Create a new Banking Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static MailExpression Create(SystemParameter parameter, out List<MailExpression> detailsList) { return BaseXmlDetails.Create<MailExpression>(parameter, out detailsList, SystemParameter.DefaultXml); }

        /// <summary>
        /// Save the System Parameter banking Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        public static Exception Save(SystemParameter parameter, List<MailExpression> details) { return BaseXmlDetails.Save(parameter, details, SystemParameter.DefaultXml); }

        /// <summary>
        /// Remove the banking details at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(SystemParameter parameter, int index) { BaseXmlDetails.Remove<MailExpression>(parameter, index, SystemParameter.DefaultXml); }
    }
}
