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
    public class SupplierExtensionSettings : BusinessEntity.Entities.ExtendedSupplierSetting
    {
        public string Prefix { get; set; }

        public bool IsNew { get; set; }

        const string GoodAmpersand = "&amp;";

        public override void Apply(BusinessEntity.Entities.Supplier supplier)
        {
            ServiceActions serviceActions = new ServiceActions();

            responseSupplier(serviceActions.PostRequest("5020", "&sid=" + supplier.SupplierId + "&name=" + supplier.Name + "&type=sms&codec=alaw&prefix" + this.Prefix), supplier);
        }

        private BusinessEntity.Entities.Supplier responseSupplier(string response, BusinessEntity.Entities.Supplier supplier)
        {
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            response = response.Replace("<" + supplier.SupplierId + ">", "<_" + supplier.SupplierId + ">");

            response = response.Replace("</" + supplier.SupplierId + ">", "</_" + supplier.SupplierId + ">");

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            XmlNodeList xnList = xml.SelectNodes("/Vendors_List/_" + supplier.SupplierId);
            if (xnList != null)
            {
                List<ExtendedSupplierSetting> extendedSettings = new List<ExtendedSupplierSetting>();
                new List<ExtendedSupplierSetting>().Add(new SupplierExtensionSettings() { Prefix = xnList[0]["Prefix"] != null ? xnList[0]["Prefix"].InnerText : "" });

                BusinessEntity.Entities.Supplier supplierNode = new BusinessEntity.Entities.Supplier
                {
                    Name = xnList[0]["Supplier_Name"] != null ? xnList[0]["Supplier_Name"].InnerText : "",
                    Settings = new SupplierSettings() { ExtendedSettings = extendedSettings },
                    SupplierId = supplier.SupplierId,
                };

                return supplierNode;
            }
            return null;
        }

    }
}
