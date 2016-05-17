using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class DatabaseScripts
    {
        public string Database { get; set; }

        public StringCollection ScriptCreateFKs { get; set; }

    }
}
