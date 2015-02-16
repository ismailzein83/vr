using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class MobileEntities
    {
        public MobileEntities(string connectionString)
            : this()
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                Database.Connection.ConnectionString = connectionString;
        }
    }
}
