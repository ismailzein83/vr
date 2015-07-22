using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        bool SaveSubscriberCase(SubscriberCase subscriberCaseObject);
    }
}
