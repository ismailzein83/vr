using Retail.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.Data.Data.SQL
{
    public class UserSessionDataManager : BaseSQLDataManager, IUserSessionDataManager
    {
        public UserSessionDataManager()
            : base(GetConnectionStringName("ICXCDRDBConnStringKey", "ICXCDRDBConnString"))
        {

        }

        public List<UserSessionData> UpdateAndGetUserSessionData(List<UserSessionData> userSessionDataList)
        {
            List<UserSessionData> results = new List<UserSessionData>();
            DataTable dtUserSessionsData = BuildUserSessionDataTable(userSessionDataList);

            ExecuteReaderSPCmd("[ICX_Data].[sp_UserSessionData_UpdateAndGetUserSessionData]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        UserSessionData userSessionData = this.UserSessionDataMapper(reader);
                        results.Add(userSessionData);
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@UserSessionDataDataTable", SqlDbType.Structured);
                    dtPrm.TypeName = "[ICX_Data].[UserSessionType]";
                    dtPrm.Value = dtUserSessionsData;
                    cmd.Parameters.Add(dtPrm);
                });

            return results;
        }

        private DataTable BuildUserSessionDataTable(List<UserSessionData> userSessionDataList)
        {
            DataTable dtUserSessionsData = new DataTable();
            dtUserSessionsData.Columns.Add("UserSession", typeof(string));
            dtUserSessionsData.Columns.Add("StartDate", typeof(DateTime));
            dtUserSessionsData.BeginLoadData();

            foreach (var userSessionData in userSessionDataList)
            {
                DataRow dr = dtUserSessionsData.NewRow();
                dr["UserSession"] = userSessionData.UserSession;
                dr["StartDate"] = userSessionData.StartDate;
                dtUserSessionsData.Rows.Add(dr);
            }

            dtUserSessionsData.EndLoadData();
            return dtUserSessionsData;
        }

        private UserSessionData UserSessionDataMapper(IDataReader reader)
        {
            return new UserSessionData()
            {
                UserSession = reader["UserSession"] as string,
                StartDate = GetReaderValue<DateTime>(reader, "StartDate")
            };
        }
    }
}
