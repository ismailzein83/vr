using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.Postgres
{
    public interface INpgBulkCopy
    {
        string ConvertToString();
    }
}
