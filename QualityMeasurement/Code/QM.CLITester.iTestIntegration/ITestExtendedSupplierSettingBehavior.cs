using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QM.BusinessEntity.Entities;
using Vanrise.Common;

namespace QM.CLITester.iTestIntegration
{
    public class ITestExtendedSupplierSettingBehavior : ExtendedSupplierSettingBehavior
    {
        public const string EXTENDEDSUPPLIERSETTING_KEYNAME = "CLISYSSupplierExtendedSettings";
        
        ServiceActions _serviceActions = new ServiceActions();

        public override void ApplyExtendedSettings(IApplyExtendedSupplierSettingsContext context)
        {
            if (context.Supplier == null)
                throw new NullReferenceException("context.Supplier");
            if (context.Supplier.Settings == null)
                throw new NullReferenceException(String.Format("context.Supplier.Settings {0}", context.Supplier.SupplierId));
            if (context.Supplier.Settings.ExtendedSettings == null)
                throw new NullReferenceException(String.Format("context.Supplier.Settings.ExtendedSettings {0}", context.Supplier.SupplierId));
            
            string matchITestSupplierId = null;
            if (context.Supplier.Settings.ExtendedSettings.Count == 0)
            {
                matchITestSupplierId = CreateSupplier();
                if(matchITestSupplierId == null)
                    throw new Exception("Could not create Supplier at ITest");
            }
            UpdateSupplier(context.Supplier);
            if (context.Supplier.Settings.ExtendedSettings.Count == 0)
            {
                var itestExtendedSettings = context.Supplier.Settings.ExtendedSettings.GetOrCreateItem(EXTENDEDSUPPLIERSETTING_KEYNAME, () => new ITestExtendedSupplierSetting()) as ITestExtendedSupplierSetting;
                itestExtendedSettings.ITestSupplierId = matchITestSupplierId;
            }
        }

        private string CreateSupplier()
        {
            string dummySupplierName = Guid.NewGuid().ToString();

            string createSupplierResponse = _serviceActions.PostRequest("5020", String.Format("&name={0}&type=std&codec=alaw", dummySupplierName));
            CheckSupplierResponse(createSupplierResponse);
            string allSuppliersResponse = _serviceActions.PostRequest("1012", null);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(allSuppliersResponse);
            foreach (XmlNode nodeSupplier in doc.DocumentElement.ChildNodes)
            {
                XmlNode nodeSupplierName = nodeSupplier.SelectSingleNode("Supplier_Name");
                if (nodeSupplierName.InnerText == dummySupplierName)
                {
                    XmlNode nodeSupplierId = nodeSupplier.SelectSingleNode("Supplier_ID");
                    return nodeSupplierId.InnerText;
                }
            }
            return null;
        }

        private void UpdateSupplier(Supplier supplier)
        {
            string createSupplierResponse = _serviceActions.PostRequest("5020", String.Format("&sid={0}&name={1}&type=std&codec=alaw&prefix={2}", supplier.SupplierId, supplier.Name, supplier.Settings.Prefix));
            CheckSupplierResponse(createSupplierResponse);
        }

        private void CheckSupplierResponse(string createSupplierResponse)
        {
            if (createSupplierResponse == null || !createSupplierResponse.Contains("<Status>Success</Status>"))
                throw new Exception(String.Format("Error when creating/updating supplier on ITest. Returned Response: {0}", createSupplierResponse));
        }
    }
}
