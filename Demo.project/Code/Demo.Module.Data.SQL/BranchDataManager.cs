using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class BranchDataManager : BaseSQLDataManager, IBranchDataManager
    {
        #region Constructors
        public BranchDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        #region Public Methods

        public List<Branch> GetBranches()
        {
            return GetItemsSP("[dbo].[sp_Branch_GetAll]", BranchMapper);
        }
        public bool Insert(Branch branch, out int insertedId)
        {
            object id;
            string serializedSetting = JsonConvert.SerializeObject(branch.Setting, settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Branch_Insert]", out id, branch.Name, branch.CompanyId, serializedSetting);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(Branch branch)
        {
            string serializedSetting = JsonConvert.SerializeObject(branch.Setting, settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Branch_Update]", branch.BranchId, branch.Name, branch.CompanyId, serializedSetting);
            return (nbOfRecordsAffected > 0);
        }
        public bool AreBranchesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Branch]", ref updateHandle);
        }
        #endregion

        #region Mappers
        Branch BranchMapper(IDataReader reader)
        {
            Branch branch = new Branch();
            branch.BranchId = GetReaderValue<int>(reader, "ID");
            
            branch.Name = GetReaderValue<string>(reader, "Name");
            branch.CompanyId = GetReaderValue<int>(reader, "CompanyID");
            branch.Setting = (reader["Setting"] as string != null ? JsonConvert.DeserializeObject<Setting>(reader["Setting"] as string, settings) : null);
            return branch;
        }
        #endregion
    }
}
