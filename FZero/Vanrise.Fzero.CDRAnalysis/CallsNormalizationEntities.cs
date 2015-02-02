using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class CallsNormalizationEntities
    {
        public CallsNormalizationEntities(string connectionString)
            : this()
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                Database.Connection.ConnectionString = connectionString;
        }
    }
}
