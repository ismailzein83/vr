﻿using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface  IAccountDataManager:IDataManager
    {
        List<Account> GetAccounts();
        bool Update(Account Account);
        bool Insert(Account Account, out int insertedId);
    }
}
