using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class BranchDataManager : BaseSQLDataManager, IBranchDataManager
    {
        #region Properties/Ctor

        public BranchDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #endregion

        #region Public Methods

        public List<Branch> GetBranches()
        {
            return GetItemsSP("[dbo].[sp_Branch_GetAll]", BranchMapper);
        }

        public bool Insert(Branch branch, out int insertedId)
        {
            string serializedBranchSettings = branch.Settings != null ? Vanrise.Common.Serializer.Serialize(branch.Settings) : null;

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Branch_Insert]", out object id, branch.Name, branch.CompanyId, serializedBranchSettings);

            bool result = nbOfRecordsAffected > 0;
            if (result)
                insertedId = (int)id;
            else
                insertedId = 0;

            return result;
        }

        public bool Update(Branch branch)
        {
            string serializedBranchSettings = branch.Settings != null ? Vanrise.Common.Serializer.Serialize(branch.Settings) : null;

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Branch_Update]", branch.BranchId, branch.Name, branch.CompanyId, serializedBranchSettings);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreBranchesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Branch]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private Branch BranchMapper(IDataReader reader)
        {
            Branch branch = new Branch();
            branch.BranchId = GetReaderValue<int>(reader, "Id");
            branch.Name = GetReaderValue<string>(reader, "Name");
            branch.CompanyId = GetReaderValue<int>(reader, "CompanyId");
            branch.Settings = Vanrise.Common.Serializer.Deserialize<BranchSettings>(reader["Settings"] as string);
            return branch;
        }

        #endregion
    }
}
