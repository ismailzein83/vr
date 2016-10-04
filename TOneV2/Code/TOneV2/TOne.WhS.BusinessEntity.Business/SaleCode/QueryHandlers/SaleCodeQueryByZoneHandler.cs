using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeQueryByZoneHandler : BaseSaleCodeQueryHandler
    {
        public SaleCodeQueryByZone Query { get; set; }

        public override IEnumerable<SaleCode> GetFilteredSaleCodes()
        {
            ISaleCodeDataManager manager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return manager.GetSaleCodesByZone(this.Query);
        }
    }
}
