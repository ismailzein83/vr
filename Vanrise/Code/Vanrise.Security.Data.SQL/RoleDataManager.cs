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
    public class RoleDataManager : BaseSQLDataManager, IRoleDataManager
    {
        public List<Entities.Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            return GetItemsSP("sec.sp_Roles_GetFiltered", RoleMapper, fromRow, toRow, name);
        }

        public Entities.Role GetRole(int roleId)
        {
            return GetItemSP("sec.sp_Roles_Get", RoleMapper, roleId);
        }

        public List<Entities.Role> GetRoles()
        {
            return GetItemsSP("sec.sp_Roles_GetAll", RoleMapper);
        }

        public List<int> GetUserRoles(int userId)
        {
            List<int> result = new List<int>();
            
            ExecuteReaderSP("sec.sp_Roles_GetUserRoles", (reader) =>
            {
                while (reader.Read())
                {
                    result.Add((int)reader["RoleID"]);
                }
            }, userId);

            return result;
        }

        public bool AddRole(Entities.Role roleObject, out int insertedId)
        {
            object roleID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Roles_Insert", out roleID, roleObject.Name,
                !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null);
            insertedId = (int)roleID;
            return (recordesEffected > 0);
        }

        public bool UpdateRole(Entities.Role roleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Roles_Update", roleObject.RoleId, roleObject.Name,
                !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null);
            return (recordesEffected > 0);
        }

        public void AssignMembers(int roleId, int [] members)
        {
            DataTable dtRoleMembers = this.BuildRoleMembersTable(roleId, members);
            int recordesEffected = ExecuteNonQuerySPCmd("sec.sp_Roles_AssignMembers", (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@RoleId", roleId));
                        var dtPrm = new SqlParameter("@UserIds", SqlDbType.Structured);
                        dtPrm.Value = dtRoleMembers;
                        cmd.Parameters.Add(dtPrm);
                    });
        }

        #region Private Methods

        Role RoleMapper(IDataReader reader)
        {
            Role role = new Role
            {
                RoleId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Description = reader["Description"] as string
            };
            return role;
        }

        DataTable BuildRoleMembersTable(int roleId, int [] members)
        {
            DataTable dtRoleMembers = new DataTable();
            dtRoleMembers.Columns.Add("UserId", typeof(int));
            dtRoleMembers.BeginLoadData();
            foreach (var userId in members)
            {
                DataRow dr = dtRoleMembers.NewRow();
                dr["UserId"] = userId;
                dtRoleMembers.Rows.Add(dr);
            }
            dtRoleMembers.EndLoadData();
            return dtRoleMembers;
        }

        #endregion
    }
}
