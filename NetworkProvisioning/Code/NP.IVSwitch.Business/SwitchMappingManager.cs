using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Business
{
    public class SwitchMappingManager
    {
        CarrierAccountManager _carrierAccountManager;

        EndPointManager _endPointManager;
        RouteManager _routeManager;
        public SwitchMappingManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _endPointManager = new EndPointManager();
            _routeManager =  new RouteManager();
        }
        public IDataRetrievalResult<SwitchMappingDetail> GetFilteredSwitchMappings(DataRetrievalInput<SwitchMappingQuery> input)
        {
            var allCarriers = _carrierAccountManager.GetAllCarriers() ;
            List<SwitchMapping> allRecords = new List<SwitchMapping>();
            foreach(var ca in allCarriers){
                allRecords.Add(new SwitchMapping(){
                    CarrierAccount = ca,
                    EndPoints = _endPointManager.GetCarrierAccountEndPointIds(ca),
                    Routes =  _routeManager.GetCarrierAccountRouteIds(ca)                
                });
            }          
            return DataRetrievalManager.Instance.ProcessResult(input, allRecords.ToBigResult(input, null, SwitchMappingDetailMapper));
        }
        #region Mappers
        SwitchMappingDetail SwitchMappingDetailMapper(SwitchMapping switchMapping)
        {
            return new SwitchMappingDetail
            {
                CarrierAccountId =  switchMapping.CarrierAccount.CarrierAccountId,
                CarrierAccountName = _carrierAccountManager.GetCarrierAccountName(switchMapping.CarrierAccount.CarrierAccountId),
                CarrierAccountType = switchMapping.CarrierAccount.AccountType,
                EndPointsDescription =switchMapping.EndPoints!=null ? GetEndPointsDescription(switchMapping.EndPoints): null,
                RoutesDescription = switchMapping.Routes!=null ? GetRoutesDescription(switchMapping.Routes) : null
            };
        }
        #endregion
        #region Privat Methode
        private string GetEndPointsDescription(List<int> endPointsIds)
        {
            List<string> description = new List<string>();
            foreach (var i in endPointsIds)
            {
                description.Add( _endPointManager.GetEndPointDescription(i));
            }
            return string.Join(", ", description);
        }

        private string GetRoutesDescription(List<int> routesIds)
        {
            List<string> description = new List<string>();
            foreach (var i in routesIds)
            {
                description.Add(_routeManager.GetRouteDescription(i));
            }
            return string.Join(", ", description);
        }
        #endregion
    }
}
