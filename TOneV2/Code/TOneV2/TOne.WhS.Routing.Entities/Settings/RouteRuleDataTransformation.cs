
using System;
namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleDataTransformation
    {
        public Guid CustomerTransformationId { get; set; }
        public Guid SupplierTransformationId { get; set; }
        public int PartialRoutesPercentageLimit { get; set; }
        public int PartialRoutesUpdateBatchSize { get; set; }
    }
}