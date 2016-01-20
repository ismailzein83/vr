using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IOrgChartDataManager : IDataManager
    {
        IEnumerable<OrgChart> GetOrgCharts();

        bool AddOrgChart(OrgChart orgChart, out int insertedId);

        bool UpdateOrgChart(OrgChart orgChart);

        bool DeleteOrgChart(int orgChartId);

        bool AreOrgChartsUpdated(ref object updateHandle);
    }
}
