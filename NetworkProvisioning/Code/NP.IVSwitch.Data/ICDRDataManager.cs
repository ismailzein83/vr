﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface ICDRDataManager : IDataManager
    {
        bool InsertHelperUser(int accountId, string logAlias);
    }
}
