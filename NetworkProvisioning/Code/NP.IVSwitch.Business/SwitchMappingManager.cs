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
            Func<SwitchMapping, bool> filterFunc = (sw) =>
            {
                if (input.Query.Name != null && !_carrierAccountManager.GetCarrierAccountName(sw.CarrierAccount).ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allRecords.ToBigResult(input, filterFunc, SwitchMappingDetailMapper));
        }

        public UpdateOperationOutput<SwitchMappingDetail> LinkCarrierToEndPoints(EndPointLink endPointLink)
        {
            var updateOperationOutput = new UpdateOperationOutput<SwitchMappingDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            _endPointManager.LinkCarrierAccountToEndPoints(endPointLink.CarrierAccountId, endPointLink.EndPointIds);
            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            SwitchMapping updatedSwitchMapping = GetSwitchMappingByCarierAccountId(endPointLink.CarrierAccountId);
            updateOperationOutput.UpdatedObject = SwitchMappingDetailMapper(updatedSwitchMapping);
            return updateOperationOutput;

        }

        public UpdateOperationOutput<SwitchMappingDetail> LinkCarrierToRoutes(RouteLink routeLink)
        {
            var updateOperationOutput = new UpdateOperationOutput<SwitchMappingDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            _routeManager.LinkCarrierAccountToRoutes(routeLink.CarrierAccountId, routeLink.RouteIds);
            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            SwitchMapping updatedSwitchMapping = GetSwitchMappingByCarierAccountId(routeLink.CarrierAccountId);
            updateOperationOutput.UpdatedObject = SwitchMappingDetailMapper(updatedSwitchMapping);
            return updateOperationOutput;
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

        SwitchMapping GetSwitchMappingByCarierAccountId(int carrierAccountId)
        {
            var carrierAccount = _carrierAccountManager.GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("CarrierAccount", carrierAccountId);
            return GetSwitchMappingByCarierAccount(carrierAccount);
        }


        SwitchMapping GetSwitchMappingByCarierAccount(CarrierAccount carrierAccount)
        {
            return new SwitchMapping()
            {
                CarrierAccount = carrierAccount,
                EndPoints = _endPointManager.GetCarrierAccountEndPointIds(carrierAccount),
                Routes = _routeManager.GetCarrierAccountRouteIds(carrierAccount)
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
