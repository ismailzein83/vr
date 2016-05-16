using QM.BusinessEntity.Entities;
using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QM.CLITester.iTestIntegration
{
    internal class CallTestManager
    {
        internal bool TryInitiateTest(string profileId, string supplierId, string countryId, string zoneId, out  InitiateTestInformation initiateTestInformation, out string failureMessage)
        {
            ServiceActions serviceActions = new ServiceActions();
            var responseString = serviceActions.PostRequest("2012", String.Format("&profid={0}&vendid={1}&ndbccgid={2}&ndbcgid={3}", profileId, supplierId, countryId, zoneId));
            

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(responseString))
            {
                xml.LoadXml(responseString);

                XmlNodeList xnList = xml.SelectNodes("/Test_Initiation/Test");
                if (xnList != null)
                {
                    if (xnList[0]["Test_ID"] != null)
                    {
                        initiateTestInformation = new InitiateTestInformation
                        {
                            Test_ID = xnList[0]["Test_ID"].InnerText
                        };
                        failureMessage = null;
                        return true;
                    }
                    else
                    {
                        initiateTestInformation = null;
                        failureMessage = responseString;
                        return false;
                    }
                }
            }
            initiateTestInformation = null;
            failureMessage = null;
            return false;
        }

        internal string GetProfileITestId(Profile profile)
        {
            return profile.SourceId;
        }

        internal string GetSupplierITestId(Supplier supplier)
        {
            ITestExtendedSupplierSetting itestSupplierSettings = null;
            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null
                && supplier.Settings.ExtendedSettings.ContainsKey(ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME))
                itestSupplierSettings = supplier.Settings.ExtendedSettings[ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME] as ITestExtendedSupplierSetting;

            if (itestSupplierSettings == null)
                throw new NullReferenceException(String.Format("itestSupplierSettings. Supplier Id '{0}'", supplier.SupplierId));
            return itestSupplierSettings.ITestSupplierId;   
        }
    }
}
