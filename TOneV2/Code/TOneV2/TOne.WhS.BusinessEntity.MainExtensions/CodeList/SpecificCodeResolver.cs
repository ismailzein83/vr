using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class SpecificCodeResolver : CodeListResolverSettings
    {
        public override Guid ConfigId { get { return new Guid("B0BBF520-A283-4051-912A-3B3DD3FCC1A2"); } }
        public List<CodeCriteria> Codes { get; set; }
        public override List<string> GetCodeList(ICodeListResolverContext context)
        {
            List<string> saleCodes = new List<string>();
            foreach (var item in this.Codes)
                saleCodes.Add(item.Code);
            return saleCodes;
           

        }


    }

}
