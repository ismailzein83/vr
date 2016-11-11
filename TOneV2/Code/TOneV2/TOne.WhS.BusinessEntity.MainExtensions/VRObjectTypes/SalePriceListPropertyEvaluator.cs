using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum SalePriceListField { PriceListDate = 0 }

    public class SalePriceListPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
