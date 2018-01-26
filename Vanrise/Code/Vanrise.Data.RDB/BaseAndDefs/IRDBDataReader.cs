using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBDataReader
    {
        bool NextResult();

        bool Read();

        object this[string name] { get; }
    }
}
