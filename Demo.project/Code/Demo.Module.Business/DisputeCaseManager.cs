using Demo.Module.Data;
using Demo.Module.Entities.DisputeCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class DisputeCaseManager
    {
        public IDataRetrievalResult<DisputeCaseDetails> GetFilteredDisputeCases(DataRetrievalInput<DisputeCaseQuery> input)
        {
            var allDisputeCases = GetCachedDisputeCases();
            Func<DisputeCase, bool> filterExpression = (disputeCase) =>
            {
                if (input.Query.CaseNumber != null && !disputeCase.CaseNumber.ToLower().Contains(input.Query.CaseNumber.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allDisputeCases.ToBigResult(input, null, DisputeCaseDetailMapper));
        }
     
        public DisputeCase GetDisputeCaseById(int disputeCaseId)
        {
            var allDisputeCases = GetCachedDisputeCases();
            return allDisputeCases.GetRecord(disputeCaseId);
        }


    
       
        private Dictionary<int, DisputeCase> GetCachedDisputeCases()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedDisputeCases", () =>
                {
                    IDisputeCaseDataManager disputeCaseDataManager = DemoModuleFactory.GetDataManager<IDisputeCaseDataManager>();
                    List<DisputeCase> disputeCases = disputeCaseDataManager.GetDisputeCases();
                    return disputeCases.ToDictionary(disputeCase => disputeCase.DisputeCaseId, disputeCase => disputeCase);
                });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDisputeCaseDataManager disputeCaseDataManager = DemoModuleFactory.GetDataManager<IDisputeCaseDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return disputeCaseDataManager.AreDisputeCasesUpdated(ref _updateHandle);
            }
        }
        public DisputeCaseDetails DisputeCaseDetailMapper(DisputeCase disputeCase)
        {
            DisputeCaseDetails disputeCaseDetails = new DisputeCaseDetails();
            disputeCaseDetails.Entity = disputeCase;
            return disputeCaseDetails;
        }
       
    }
}
