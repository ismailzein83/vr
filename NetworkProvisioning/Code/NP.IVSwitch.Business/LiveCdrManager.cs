using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace NP.IVSwitch.Business
{
    public class LiveCdrManager
    {
       
        public Vanrise.Entities.IDataRetrievalResult<LiveCdrItemDetail> GetFilteredLiveCdrs(Vanrise.Entities.DataRetrievalInput<LiveCdrQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new LiveCdrRequestHandler());
        }

        private class LiveCdrRequestHandler : BigDataRequestHandler<LiveCdrQuery, LiveCdrItem, LiveCdrItemDetail>
        {
            EndPointManager endPointManager = new EndPointManager();
            RouteManager routeManager = new RouteManager();
            public override LiveCdrItemDetail EntityDetailMapper(LiveCdrItem entity)
            {
                return new LiveCdrItemDetail()
                {
                    customerName = endPointManager.GetEndPointAccountName(entity.customerId),
                  sourceIP =entity.sourceIP,
                 attemptDate =entity.attemptDate,
                 cli =entity.cli,
                 destinationCode=entity.destinationCode,
                 destinationName =entity.destinationName,
                    supplierName = routeManager.GetRouteCarrierAccountName(entity.routeId),
                 routeIP=   entity.routeIP,
                 supplierCode=entity.supplierCode,
                 supplierZone=entity.supplierZone,   
                 alertDate=entity.alertDate,
                 connectDate=   entity.connectDate
                };
            }

            public override IEnumerable<LiveCdrItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<LiveCdrQuery> input)
            {
                ICDRDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICDRDataManager>();
                Helper.SetSwitchConfig(dataManager);
                return dataManager.GetFilteredLiveCdrs(input.Query.EndPointIds,input.Query.RouteIds,input.Query.SourceIP,input.Query.RouteIP);

            }


        }
    }
}
