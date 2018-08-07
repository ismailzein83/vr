using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class SpecificCountryCodeListResolver : CodeListResolverSettings
    {
        public override Guid ConfigId { get { return new Guid("A3B95375-BB41-4843-A29D-532E402A2421"); } }
        public List<int> CountryIds { get; set; }
        public override List<string> GetCodeList(ICodeListResolverContext context)
        {
            throw new NotImplementedException();
        }
    }
}
