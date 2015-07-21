using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Data
{
    public interface IOrgChartLinkedEntityDataManager : IDataManager
    {
        int? GetLinkedOrgChartId(string linkedEntityIdentifier);

        void InsertOrUpdate(int orgChartId, string linkedEntityIdentifier);
    }
}
