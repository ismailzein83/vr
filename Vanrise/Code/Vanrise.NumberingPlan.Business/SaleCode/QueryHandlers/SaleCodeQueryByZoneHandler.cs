using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class SaleCodeQueryByZoneHandler : BaseSaleCodeQueryHandler
    {
        public SaleCodeQueryByZone Query { get; set; }

        public override IEnumerable<SaleCode> GetFilteredSaleCodes()
        {
            ISaleCodeDataManager manager = CodePrepDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return manager.GetSaleCodesByZone(this.Query);
        }
    }
}
