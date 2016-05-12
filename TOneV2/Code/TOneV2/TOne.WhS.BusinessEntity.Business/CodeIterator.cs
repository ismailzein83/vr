using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeIterator<T> : VRCodeIterator<T>
        where T : ICode
    {
        public CodeIterator(IEnumerable<T> codeObjects)
            : base(codeObjects)
        {
        }

        protected override List<string> GetCodes(T codeObject)
        {
            if (!String.IsNullOrEmpty(codeObject.Code))
                return new List<string> { codeObject.Code };
            else
                return null;
        }
    }
}
