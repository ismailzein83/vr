﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountBEDataManager : IDataManager
    {
        IEnumerable<Account> GetAccounts(Guid accountBEDefinitionId);

        bool Insert(AccountToInsert accountToInsert, out long insertedId);

        bool Update(AccountToEdit accountToEdit, long? parentId);

        bool AreAccountsUpdated(Guid accountBEDefinitionId, ref object updateHandle);
    }
}
