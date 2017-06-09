using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public abstract class ParsedRecord
    {
        public abstract void SetFieldValue(string fieldName, Object value);
        public abstract Object GetFieldValue(string fieldName);

    }
}
