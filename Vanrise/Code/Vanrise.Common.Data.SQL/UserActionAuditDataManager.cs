using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class UserActionAuditDataManager : BaseSQLDataManager, IUserActionAuditDataManager
    {

        public UserActionAuditDataManager()
            : base(GetConnectionStringName("LogDBConnkey", "LogDBConnString"))
        {

        }

        public void Insert(UserActionAudit userActionAudit)
        {


            ExecuteNonQuerySP("[logging].[sp_UserActionAudit_Insert]",
                        userActionAudit.UserId,
                        userActionAudit.ModuleName,
                        userActionAudit.ControllerName,
                        userActionAudit.ActionName,
                        userActionAudit.BaseUrl
                );
        }

        public List<UserActionAudit> GetAll(UserActionAuditQuery query)
        {
            string userIds= null;
            if(query.UserIds != null && query.UserIds.Count > 0)
                userIds =  string.Join(",", query.UserIds);
            return GetItemsSP("logging.sp_UserActionAudit_GetFiltered", UserActionAuditMapper,
                    query.TopRecord,
                    userIds,
                    query.Module,
                    query.Controller,
                    query.Action,
                    query.BaseUrl,
                    query.FromTime,
                    query.ToTime
                );
        }


        private UserActionAudit UserActionAuditMapper(IDataReader reader)
        {
            UserActionAudit userActionAudit = new UserActionAudit
            {
                UserActionAuditId = (long)reader["ID"],
                UserId = GetReaderValue<int?>(reader,"UserId"),
                ModuleName = reader["ModuleName"] as string,
                ControllerName = reader["ControllerName"] as string,
                ActionName = reader["ActionName"] as string,
                BaseUrl = reader["BaseUrl"] as string,
                LogTime = GetReaderValue<DateTime>(reader, "LogTime")

            };

            return userActionAudit;
        }

    }
}
