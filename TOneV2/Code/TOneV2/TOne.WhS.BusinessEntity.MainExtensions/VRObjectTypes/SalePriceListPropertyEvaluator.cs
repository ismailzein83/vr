using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum SalePriceListField { PriceListDate = 0, PriceListType = 1 ,CurrencySymbol = 2}

    public class SalePriceListPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("FD906B70-787B-48AB-9F8F-5B86169E7036"); }
        }
        public SalePriceListField SalePriceListField { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SalePriceList salePriceList = context.Object as SalePriceList;

            if (salePriceList == null)
                throw new NullReferenceException("Sale Pricelist");

            switch (this.SalePriceListField)
            {
                case MainExtensions.SalePriceListField.PriceListDate:
                    return salePriceList.CreatedTime;
                case MainExtensions.SalePriceListField.PriceListType:
                    return salePriceList.PriceListType;
                case MainExtensions.SalePriceListField.CurrencySymbol:
                    return currencyManager.GetCurrencySymbol(salePriceList.CurrencyId);

            }

            return null;
        }
    }
}
