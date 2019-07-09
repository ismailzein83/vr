using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricelistSettings
    {
        public Guid? DefaultSalePLMailTemplateId { get; set; }
        public int? DefaultSalePLTemplateId { get; set; }
        public PriceListExtensionFormat? PriceListExtensionFormat { get; set; }
        public SalePriceListType? PriceListType { get; set; }
        public bool? CompressPriceListFile { get; set; }
        public CodeChangeTypeDescriptions CodeChangeTypeDescriptions { get; set; }
        public RateChangeTypeDescriptions RateChangeTypeDescriptions { get; set; }
        public string SalePricelistFileNamePattern { get; set; }
        public IncludeClosedEntitiesEnum? IncludeClosedEntities { get; set; }
        public string SubjectCode { get; set; }
    }
    public class PurchasePricelistSettings
    {
        public Guid? DefaultSupplierPLMailTemplateId { get; set; }
        public SendEmail SendEmail { get; set; }
    }
    public enum SendEmail { Yes = 0, No = 1 }
}
