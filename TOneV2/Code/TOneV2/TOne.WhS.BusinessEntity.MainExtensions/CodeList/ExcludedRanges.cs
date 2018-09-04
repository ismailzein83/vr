using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class ExcludedRanges : ExcludedDestinations
    {
        public override Guid ConfigId { get { return new Guid("0904D57D-13B5-4F07-A0F5-05339FBFB2B3"); } }

        public List<CodeRange> CodeRanges { get; set; }
        public override List<string> GetExcludedCodes(ICodeListExcludedContext context)
        {
            List<string> codes = new List<string>();
            foreach(var item in CodeRanges)
            {
                int from;
                int to;
                int.TryParse(item.FromCode, out from);
                int.TryParse(item.ToCode, out to);
                for (; from < to; from++)
                    codes.Add(from.ToString());

                

            }
            return codes;

        }
    }
    public class CodeRange
    {
        public string FromCode { get; set; }

        public string ToCode { get; set; }
    }
}
