using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;
using System.Net;
using System.IO;
using QM.CLITester.Entities;
using System.Text.RegularExpressions;
using System.Xml;
//using QM.BusinessEntity.Data;
//using Vanrise.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class SupplierExtensionSettings : ExtendedSupplierSetting
    {
        public string Prefix { get; set; }        

        public string ITestSupplierId { get; set; }

        public override void Apply(Supplier supplier)
        {
            if(this.ITestSupplierId == null)
            {
                this.ITestSupplierId = CreateSupplier();
                if (this.ITestSupplierId == null)
                    throw new Exception("Could not create Supplier at ITest");
            }
            UpdateSupplier(supplier);
        }
        ServiceActions _serviceActions = new ServiceActions();
        private string CreateSupplier()
        {
            string dummySupplierName = Guid.NewGuid().ToString();
            
            string createSupplierResponse = _serviceActions.PostRequest("5020", String.Format("&name={0}&type=std&codec=alaw", dummySupplierName));
            CheckSupplierResponse(createSupplierResponse);
            string allSuppliersResponse = _serviceActions.PostRequest("1012", null);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(allSuppliersResponse);
            foreach(XmlNode nodeSupplier in doc.DocumentElement.ChildNodes)
            {
                XmlNode nodeSupplierName = nodeSupplier.SelectSingleNode("Supplier_Name");
                if(nodeSupplierName.InnerText == dummySupplierName)
                {
                    XmlNode nodeSupplierId = nodeSupplier.SelectSingleNode("Supplier_ID");
                    return nodeSupplierId.InnerText;
                }
            }
            return null;
        }

        private void CheckSupplierResponse(string createSupplierResponse)
        {
            if (createSupplierResponse == null || !createSupplierResponse.Contains("<Status>Success</Status>"))
                throw new Exception(String.Format("Error when creating/updating supplier on ITest. Returned Response: {0}", createSupplierResponse));
        }

        private void UpdateSupplier(Supplier supplier)
        {
            string createSupplierResponse = _serviceActions.PostRequest("5020", String.Format("&sid={0}&name={1}&type=std&codec=alaw&prefix={2}", this.ITestSupplierId, supplier.Name, this.Prefix));
            CheckSupplierResponse(createSupplierResponse);
        }

        const string EXCELFIELD_PREFIX = "Prefix";
        public override string[] GetExcelColumnNames()
        {
            return new string[] { EXCELFIELD_PREFIX };
        }

        public override void ApplyExcelFields(Supplier supplier, Dictionary<string, object> excelFields)
        {
            object prefix;
            if (excelFields.TryGetValue(EXCELFIELD_PREFIX, out prefix))
                this.Prefix = prefix as string;
        }
    }

}
