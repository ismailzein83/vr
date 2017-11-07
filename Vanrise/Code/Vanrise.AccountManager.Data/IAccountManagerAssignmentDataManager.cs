﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Data
{
    public interface IAccountManagerAssignmentDataManager : IDataManager
    {
        List<Entities.AccountManagerAssignment> GetAccountManagerAssignments();
        bool AreAccountManagerAssignmentsUpdated(ref object updateHandle);
        bool AddAccountManagerAssignment( AccountManagerAssignment accountManagerAssignment, out int insertedId);
        bool UpdateAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment);
    }
}
