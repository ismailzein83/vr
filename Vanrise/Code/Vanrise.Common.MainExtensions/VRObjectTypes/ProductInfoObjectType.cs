using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class ProductInfoObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("4CD9F093-41D3-42AE-B878-9AFBDB656262"); } }
        public override dynamic GetDefaultValue()
        {
            ProductInfoTechnicalSettings productInfoTechnicalSettings = new ProductInfoTechnicalSettings();

            ProductInfo productInfo = new ConfigManager().GetProductInfo();

            return productInfo;
        }
    }
}
