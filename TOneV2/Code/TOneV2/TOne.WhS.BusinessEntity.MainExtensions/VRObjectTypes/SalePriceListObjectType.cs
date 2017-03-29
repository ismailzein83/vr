using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SalePriceListObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("D45831D4-9818-44E6-B350-BF83017F5D9E"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            SalePriceList salePriceList = salePriceListManager.GetPriceList((int)context.ObjectId);

            if (salePriceList == null)
                throw new DataIntegrityValidationException(string.Format("Sale Price List not found for ID: '{0}'", context.ObjectId));

            return salePriceList;
        }
    }
}
