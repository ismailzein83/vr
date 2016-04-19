using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum LogEntryType
    {
        Error = 1,
        Warning = 2,
        Information = 4,
        Verbose = 8
    }
}
