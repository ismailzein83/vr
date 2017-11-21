using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Data
{
    public interface IAccountManagerAssignmentDataManager : IDataManager
    {
        List<Entities.AccountManagerAssignment> GetAccountManagerAssignments(Guid accountManagerAssignmentDefinitionId);
        bool AreAccountManagerAssignmentsUpdated(Guid accountManagerDefinitionId, ref object updateHandle);
        bool AddAccountManagerAssignment( AccountManagerAssignment accountManagerAssignment, out int insertedId);
        bool UpdateAccountManagerAssignment(long accountManagerAssignmentId, DateTime bed, DateTime? eed, AccountManagerAssignmentSettings settings);
    }
}
