using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IRegisteredApplicationDataManager : IDataManager
    {
        List<RegisteredApplication> GetRegisteredApplications();
        bool AddRegisteredApplication(RegisteredApplication registeredApplication);
        bool AreRegisteredApplicationsUpdated(ref object updateHandle);
    }
}