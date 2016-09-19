using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public enum ProductField { ProductName = 0, VersionNumber = 1 }

    public class ProductInfoPropertyEvaluator : VRObjectPropertyEvaluator
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("385C968A-415A-49E2-B7EF-189C2A6DD484"); } }
        public ProductField ProductField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            ProductInfo productInfo = context.Object as ProductInfo;

            if (productInfo == null)
                throw new NullReferenceException("productInfo");

            switch (this.ProductField)
            {
                case ProductField.ProductName: return productInfo.ProductName;

                case ProductField.VersionNumber: return productInfo.VersionNumber;
            }

            return null;
        }
    }
}
