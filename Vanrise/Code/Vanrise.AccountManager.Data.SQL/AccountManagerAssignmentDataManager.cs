using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.AccountManager.Entities;
using Vanrise.Common;

namespace Vanrise.AccountManager.Data.SQL
{
    public class AccountManagerAssignmentDataManager : BaseSQLDataManager, IAccountManagerAssignmentDataManager
    {
        public AccountManagerAssignmentDataManager()
           : base(GetConnectionStringName("VR_AccountManager_AccountManagerDBConnStringKey", "VR_AccountManager_AccountManagerDBConnString"))
    {}
        public List<Vanrise.AccountManager.Entities.AccountManagerAssignment> GetAccountManagerAssignments()
        {
            return GetItemsSP("[VR_AccountManager].[sp_AccountManagerAssignment_GetAll]", AccountManagerAssignmentMapper);
        }
        public bool AddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment, out int insertedId)
        {
            object accountManagerAssignmentId;
            int recordsEffected = ExecuteNonQuerySP("[VR_AccountManager].[sp_AccountManagerAssignment_Insert]", out accountManagerAssignmentId, accountManagerAssignment.AccountManagerAssignementDefinitionId, accountManagerAssignment.AccountManagerId, accountManagerAssignment.AccountId, Serializer.Serialize(accountManagerAssignment.Settings), accountManagerAssignment.BED, accountManagerAssignment.EED);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
            {
                insertedId = (int)accountManagerAssignmentId;
            }
            else
            {
                insertedId = 0;
            }
            return insertedSuccesfully;
        }
        public bool UpdateAccountManagerAssignment(long accountManagerAssignmentId, DateTime bed, DateTime? eed, AccountManagerAssignmentSettings settings)
        {
            int recordsEffected = ExecuteNonQuerySP("[VR_AccountManager].[sp_AccountManagerAssignment_Update]",accountManagerAssignmentId, Serializer.Serialize( settings), bed,eed);
            return (recordsEffected > 0);
        }
        public bool AreAccountManagerAssignmentsUpdated(Guid accountManagerDefinitionId,ref object updateHandle)
        {
            return base.IsDataUpdated("VR_AccountManager.AccountManagerAssignment", "AssignmentDefinitionID", accountManagerDefinitionId, ref updateHandle);
        }
        Vanrise.AccountManager.Entities.AccountManagerAssignment AccountManagerAssignmentMapper(IDataReader reader)
        {
            return new Vanrise.AccountManager.Entities.AccountManagerAssignment()
            {
                AccountManagerAssignementId = GetReaderValue<long>(reader, "ID"),
                AccountManagerAssignementDefinitionId = GetReaderValue<Guid>(reader, "AssignmentDefinitionID"),
                AccountManagerId = GetReaderValue<long>(reader, "AccountManagerID"),
                AccountId = reader["AccountID"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<Vanrise.AccountManager.Entities.AccountManagerAssignmentSettings>(reader["Settings"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }
    }
}
