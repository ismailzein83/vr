﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWAccountCaseManager:IDataManager
    {
        List<DWAccountCase> GetDWAccountCases(DateTime from, DateTime to);
    }
}
