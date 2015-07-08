using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class OrgChartDataManager : BaseSQLDataManager, IUserDataManager
    {
        public OrgChartDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        private OrgChart OrgChartMapper(IDataReader reader)
        {
            return new OrgChart
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Hierarchy = reader["Hierarchy"] as string,
            };
        }

        public List<OrgChart> GetFilteredOrgCharts(int fromRow, int toRow, string name)
        {
            return GetItemsSP("sec.sp_OrgChart_GetFiltered", OrgChartMapper, fromRow, toRow, name);
        }
    }
}
