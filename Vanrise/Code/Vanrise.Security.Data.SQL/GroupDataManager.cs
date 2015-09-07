using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class GroupDataManager : BaseSQLDataManager, IGroupDataManager
    {
        public GroupDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Group> GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("sec.sp_Group_CreateTempByName", tempTableName, input.Query.Name);
            };

            return RetrieveData(input, createTempTableAction, GroupMapper);
        }

        public Entities.Group GetGroup(int groupId)
        {
            return GetItemSP("sec.sp_Group_Get", GroupMapper, groupId);
        }

        public List<Entities.Group> GetGroups()
        {
            return GetItemsSP("sec.sp_Group_GetAll", GroupMapper);
        }

        public List<int> GetUserGroups(int userId)
        {
            List<int> result = new List<int>();
            
            ExecuteReaderSP("sec.sp_Group_GetUserGroups", (reader) =>
            {
                while (reader.Read())
                {
                    result.Add((int)reader["GroupId"]);
                }
            }, userId);

            return result;
        }

        public bool AddGroup(Entities.Group groupObj, out int insertedId)
        {
            object groupID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Group_Insert", out groupID, groupObj.Name, groupObj.Description);

            insertedId = (recordesEffected > 0) ? (int)groupID : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateGroup(Entities.Group groupObj)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Group_Update", groupObj.GroupId, groupObj.Name, groupObj.Description);
            return (recordesEffected > 0);
        }

        public void AssignMembers(int groupId, int [] members)
        {
            DataTable dtGroupMembers = this.BuildGroupMembersTable(groupId, members);
            int recordesEffected = ExecuteNonQuerySPCmd("sec.sp_Group_AssignMembers", (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@GroupId", groupId));
                        var dtPrm = new SqlParameter("@UserIds", SqlDbType.Structured);
                        dtPrm.Value = dtGroupMembers;
                        cmd.Parameters.Add(dtPrm);
                    });
        }

        #region Private Methods

        Group GroupMapper(IDataReader reader)
        {
            Group group = new Group
            {
                GroupId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Description = reader["Description"] as string
            };

            return group;
        }

        DataTable BuildGroupMembersTable(int groupId, int [] members)
        {
            DataTable dtGroupMembers = new DataTable();
            dtGroupMembers.Columns.Add("UserId", typeof(int));
            dtGroupMembers.BeginLoadData();
            foreach (var userId in members)
            {
                DataRow dr = dtGroupMembers.NewRow();
                dr["UserId"] = userId;
                dtGroupMembers.Rows.Add(dr);
            }
            dtGroupMembers.EndLoadData();
            return dtGroupMembers;
        }

        #endregion
    }
}
