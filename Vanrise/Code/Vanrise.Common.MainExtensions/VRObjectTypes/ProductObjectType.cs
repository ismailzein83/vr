using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class ProductObjectType : VRObjectType
    {
        public override dynamic GetDefaultValue()
        {
            ProductInfoTechnicalSettings productInfoTechnicalSettings = new ProductInfoTechnicalSettings();

            productInfoTechnicalSettings.ProductName = new ConfigManager().GetProductName();
            productInfoTechnicalSettings.VersionNumber = new ConfigManager().GetProductVersionNumber();

            return productInfoTechnicalSettings;
        }
    }
}
