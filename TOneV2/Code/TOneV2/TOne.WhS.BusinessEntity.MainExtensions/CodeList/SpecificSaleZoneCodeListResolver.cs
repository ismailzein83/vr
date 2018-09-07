using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class SpecificSaleZoneCodeListResolver : CodeListResolverSettings
    {
        public override Guid ConfigId { get { return new Guid("054B8A6C-1FFF-4BC6-8620-8A942A5980B6"); } }
        public List<long> ZoneIds { get; set; }
        public ExcludedDestinations ExcludedDestinations { get; set; }

        public override List<string> GetCodeList(ICodeListResolverContext context)
        {
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            List<string> codesList=new List<string>();
            List<string> excludedCodes = (ExcludedDestinations != null) ? this.ExcludedDestinations.GetExcludedCodes(new CodeListExcludedContext()) : new List<string>();
            foreach (var item in saleCodeManager.GetSaleCodesByZoneIDs(this.ZoneIds, DateTime.Now))
                if(!excludedCodes.Contains(item.Code))
                codesList.Add(item.Code);
            return codesList;
        }

    }
}
