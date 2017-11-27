using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
