using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class SpecificSaleZoneCodeListResolver : CodeListResolverSettings
    {
        public override Guid ConfigId { get { return new Guid("054B8A6C-1FFF-4BC6-8620-8A942A5980B6"); } }
        public List<long> ZoneIds { get; set; }
        public override List<string> GetCodeList(ICodeListResolverContext context)
        {
            throw new NotImplementedException();
        }
    }
}
