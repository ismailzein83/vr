﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRunningProcessDataManager : IDataManager
    {
        RunningProcessInfo InsertProcessInfo(Guid runtimeNodeId, Guid runtimeNodeInstanceId, int osProcessId, RunningProcessAdditionalInfo additionalInfo);

        List<RunningProcessInfo> GetRunningProcesses();

        void DeleteRunningProcess(int runningProcessId);
        List<Entities.RunningProcessDetails> GetFilteredRunningProcesses(DataRetrievalInput<RunningProcessQuery> input);

        bool IsExists(int runningProcessId);
    }
}
