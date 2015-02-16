using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class Entities
    {
        public Entities(string connectionString)
            : this()
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                Database.Connection.ConnectionString = connectionString;
        }
    }
}
