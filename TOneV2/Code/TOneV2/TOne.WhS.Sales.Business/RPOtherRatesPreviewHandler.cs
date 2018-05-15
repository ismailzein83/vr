using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
   public class RPOtherRatesPreviewHandler : BaseOtherRatesPreviewQueryHandler
    {
       public RPOtherRatesPreviewQuery Query { get; set; }

       public override IEnumerable<SalePricelistRateChange> GetFilteredRatesPreview()
       {
           var query = this.Query;
           IOtherRatesPreviewDataManager dataManager = BEDataManagerFactory.GetDataManager<IOtherRatesPreviewDataManager>();
           return dataManager.GetFilteredRatesPreviewByProcessInstanceId(query.ProcessInstanceId, query.ZoneName, query.CustomerId);
       }
    }
}
