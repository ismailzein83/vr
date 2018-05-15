using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class OtherRatesPreviewHandler : BaseOtherRatesPreviewQueryHandler
    {
       public OtherRatesPreviewQuery Query { get; set; }

       public override IEnumerable<SalePricelistRateChange> GetFilteredRatesPreview()
       {
           IOtherRatesPreviewDataManager dataManager = BEDataManagerFactory.GetDataManager<IOtherRatesPreviewDataManager>();
           return dataManager.GetFilteredRatesPreviewByPriceListId(this.Query.PriceListId,this.Query.ZoneName);
       }
    }
}
