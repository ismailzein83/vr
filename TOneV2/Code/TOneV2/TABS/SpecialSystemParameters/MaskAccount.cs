using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace TABS.SpecialSystemParameters
{
    [Serializable]
    public class MaskAccount
    {
        public MaskAccount()
        {
            Accounts = new List<CarrierAccount>();
        }


        protected List<TABS.CarrierAccount> _Accounts;
        public List<TABS.CarrierAccount> Accounts
        {
            get { return _Accounts; }
            set { _Accounts = value; }
        }

        public static MaskAccount SystemMask
        {
            get { return (MaskAccount)DeSerializeObject(TABS.SystemParameter.MaskCarrierAccount.LongTextValue, typeof(MaskAccount)); }
        }

        protected static string SerializeObject(Object obj)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(sb);
            ser.Serialize(writer, obj);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            return doc.InnerXml;
        }
        protected static object DeSerializeObject(string documentvalue, Type objType)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(documentvalue);
            XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
            XmlSerializer ser = new XmlSerializer(objType);
            object obj = ser.Deserialize(reader);
            return obj;
        }

        [NonSerialized]
        internal static Dictionary<string, TABS.CarrierAccount> _AllMasks = new Dictionary<string, CarrierAccount>();
        public static Dictionary<string, TABS.CarrierAccount> AllMasks
        {
            get
            {
                var maskAccount = (MaskAccount)DeSerializeObject(TABS.SystemParameter.MaskCarrierAccount.LongTextValue, typeof(MaskAccount));
                _AllMasks = maskAccount.Accounts.ToDictionary(c => c.CarrierAccountID);
                return _AllMasks;
            }
        }

        public static bool Save(MaskAccount maskAccount, out Exception ex)
        {
            bool succes = true;
            TABS.SystemParameter.MaskCarrierAccount.LongTextValue = SerializeObject(maskAccount);
            try
            {
                ObjectAssembler.SaveOrUpdate(TABS.SystemParameter.MaskCarrierAccount, out ex);
            }
            catch (Exception exp)
            {
                ex = exp;
                succes = false;
            }
            return succes;
        }

        public static string DefaultXml
        {
            get
            {
                var dummyMask = new MaskAccount();
                TABS.CarrierAccount carrierAccount = new TABS.CarrierAccount();
                TABS.CarrierProfile carrierProfile = new TABS.CarrierProfile();
                carrierProfile.CompanyName = "Other";
                carrierProfile.Country = "Other";
                carrierProfile.Name = "Other";
                carrierAccount.CarrierProfile = carrierProfile;
                carrierAccount.CarrierAccountID = "Default";
                carrierProfile.Country = TABS.Lookups.LookupType.All["Countries"].Values.First().Value;
                carrierProfile.Currency = TABS.Currency.Main;
                dummyMask.Accounts.Add(carrierAccount);
                var xmlSerialized = SerializeObject(dummyMask);
                return xmlSerialized;
            }
        }

        public static string GetMaskID
        {
            get
            {
                if (SystemMask.Accounts.Count() == 0 || !SystemMask.Accounts.Last().CarrierAccountID.Contains("_"))
                    return "Msk_1";
                else
                    return "Msk_" + (int.Parse(SystemMask.Accounts.Last().CarrierAccountID.Split('_')[1]) + 1);
            }
        }

    }

}
