using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
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
