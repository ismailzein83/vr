using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class OrgChartManager
    {
        public List<Vanrise.Security.Entities.OrgChart> GetFilteredOrgCharts(int fromRow, int toRow, string name)
        {
            IOrgChartDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return datamanager.GetFilteredOrgCharts(fromRow, toRow, name);
        }
    }
}
