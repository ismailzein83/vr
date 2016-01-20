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
    public class OrgChartDataManager : BaseSQLDataManager, IOrgChartDataManager
    {
        public OrgChartDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #region Public Methods

        public IEnumerable<OrgChart> GetOrgCharts()
        {
            return GetItemsSP("sec.sp_OrgChart_GetAll", OrgChartMapper);
        }

        public bool AddOrgChart(OrgChart orgChartObject, out int insertedId)
        {
            object orgChartID;

            // serialize orgChartObject.Hierarchy
            string serializedHierarchy = Vanrise.Common.Serializer.Serialize(orgChartObject.Hierarchy, true);

            int recordsEffected = ExecuteNonQuerySP("sec.sp_OrgChart_Insert", out orgChartID, orgChartObject.Name, serializedHierarchy);

            insertedId = (recordsEffected > 0) ? (int)orgChartID : -1;

            return (recordsEffected > 0);
        }

        public bool UpdateOrgChart(OrgChart orgChartObject)
        {
            // serialize orgChartObject.Hierarchy
            string serializedHierarchy = Vanrise.Common.Serializer.Serialize(orgChartObject.Hierarchy, true);

            int recordsEffected = ExecuteNonQuerySP("sec.sp_OrgChart_Update", orgChartObject.OrgChartId, orgChartObject.Name, serializedHierarchy);
            return (recordsEffected > 0);
        }

        public bool DeleteOrgChart(int orgChartId)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_OrgChart_Delete", orgChartId);
            return (recordsEffected > 0);
        }

        public bool AreOrgChartsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.OrgChart", ref updateHandle);
        }
        
        #endregion

        #region Mappers

        OrgChart OrgChartMapper(IDataReader reader)
        {
            return new OrgChart
            {
                OrgChartId = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Hierarchy = Vanrise.Common.Serializer.Deserialize<List<Member>>(reader["Hierarchy"] as string),
            };
        }
        
        #endregion
    }
}
