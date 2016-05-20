using System.Collections.Specialized;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class DatabaseScripts
    {
        public string Database { get; set; }

        public StringCollection ScriptCreateFKs { get; set; }

    }
}
