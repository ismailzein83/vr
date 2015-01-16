using System;
using System.Collections.Generic;

namespace TABS.SpecialSystemParameters
{
    public class EmailDetails : BaseXmlDetails
    {

        public MailTemplateType Type { get { return (MailTemplateType)Enum.Parse(typeof(MailTemplateType), Get("Type")); } set { Set("Type", value.ToString()); } }
        public string From { get { return Get("From"); } set { Set("From", value); } }
        public string To { get { return Get("To"); } set { Set("To", value); } }
        public string CC { get { return Get("CC"); } set { Set("CC", value); } }
        public string Bcc { get { return Get("Bcc"); } set { Set("Bcc", value); } }
        public string Subject { get { return Get("Subject"); } set { Set("Subject", value); } }
        public string SMS { get { return Get("SMS"); } set { Set("SMS", value); } }
        public string Body { get { return Get("Body"); } set { Set("Body", value); } }

        /// <summary>
        /// Returns the list of available email details from the system parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<EmailDetails> Get(SystemParameter parameter) { return BaseXmlDetails.Get<EmailDetails>(parameter); }

        /// <summary>
        /// Create a new Detail
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EmailDetails Create(SystemParameter parameter, out List<EmailDetails> detailsList) { return BaseXmlDetails.Create<EmailDetails>(parameter, out detailsList, EmailDetails.DefaultXml); }

        /// <summary>
        /// Save the System Parameter for Email Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="details"></param>
        public static Exception Save(SystemParameter parameter, List<EmailDetails> details) { return BaseXmlDetails.Save(parameter, details, EmailDetails.DefaultXml); }

        /// <summary>
        /// Remove the email details at the given index (if they exist)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="index"></param>
        public static void Remove(SystemParameter parameter, int index) { BaseXmlDetails.Remove<EmailDetails>(parameter, index, EmailDetails.DefaultXml); }

        public static EmailDetails GetMailTemplateOf(TABS.MailTemplateType type)
        {
            List<EmailDetails> mailList = EmailDetails.Get(SystemParameter.MailDetails);
            EmailDetails mailDetail = null;
            foreach (EmailDetails detail in mailList)
            {
                if (detail.Type == type)
                {
                    mailDetail = detail;
                    break;
                }
            }

            return mailDetail;
        }

        public static readonly string
            DefaultXml =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <SystemParameter>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.Alert.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.Invoice.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.InvoiceSupplier.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.Pricelist.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.TroubleTicketIN.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.TroubleTicketOUT.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.TroubleTicketCloseOUT.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PricelistImport.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.InvoiceGeneration.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.TroubleTicketInUpdate.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.TroubleTicketOutUpdate.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.EATroubleTicketIN.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.EATroubleTicketOUT.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>      
                                    <Type><![CDATA[" + MailTemplateType.EATroubleTicketInUpdate.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.EATroubleTicketOutUpdate.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PrepaidCustomerAction.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PrepaidSupplierAction.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PostpaidCustomerAction.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PostpaidSupplierAction.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PrepaidSMSCustomer.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PostpaidSMSCustomer.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.PostpaidSMSSupplier.ToString() + @"]]></Type>
                            </EmailDetails>
                            <EmailDetails>
                                    <Type><![CDATA[" + MailTemplateType.UserCreationNotification + @"]]></Type>
                            </EmailDetails>
                    </SystemParameter>";
    }
}