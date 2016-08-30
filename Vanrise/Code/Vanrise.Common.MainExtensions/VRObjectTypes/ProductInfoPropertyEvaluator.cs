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
