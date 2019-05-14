using System;
using System.Collections.Generic;

namespace Vanrise.Common
{
    public class StringCodeIterator : VRCodeIterator<string>
    {
        public StringCodeIterator(IEnumerable<string> codeObjects)
            : base(codeObjects)
        {
        }

        protected override List<string> GetCodes(string codeObject)
        {
            if (!String.IsNullOrEmpty(codeObject))
                return new List<string> { codeObject };
            else
                return null;
        }
    }
}