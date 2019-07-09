using System;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CompanyPricelistSettings : BaseCompanyExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("48AB58C0-8D37-4F6A-A5EC-5EAFE9AAA586"); } }
        public PricelistSettings PricelistSettings { get; set; }

    }
    public class CompanyPurchasePricelistSettings : BaseCompanyExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("19312F80-55D4-41BA-AA04-14E4C8101787"); } }
        public PurchasePricelistSettings PricelistSettings { get; set; }

    }
}
