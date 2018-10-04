using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IProcessSynchronisationDataManager : IDataManager
    {
        List<ProcessSynchronisation> GetProcessSynchronisations();

        bool InsertProcessSynchronisation(Guid processSynchronisationId, ProcessSynchronisationToAdd processSynchronisationToAdd, int createdBy);

        bool UpdateProcessSynchronisation(ProcessSynchronisationToUpdate processSynchronisationToUpdate, int lastModifiedBy);

        bool AreProcessSynchronisationsUpdated(ref object updateHandle);

        bool EnableProcessSynchronisation(Guid processSynchronisationId, int lastModifiedBy);

        bool DisableProcessSynchronisation(Guid processSynchronisationId, int lastModifiedBy);
    }
}