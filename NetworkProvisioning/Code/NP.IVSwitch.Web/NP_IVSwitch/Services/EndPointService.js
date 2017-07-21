
(function (appControllers) {

    "use strict";

    EndPointService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService', 'NP_IVSwitch_CarrierAccountTypeEnum', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function EndPointService(NPModalService, WhS_BE_CarrierAccountService, NP_IVSwitch_CarrierAccountTypeEnum, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addEndPoint(CarrierAccountId, onEndPointAdded) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onEndPointAdded = onEndPointAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/EndPoint/EndPointEditor.html', parameters, settings);
        };
        function editEndPoint(EndPointId, onEndPointUpdated) {
            var settings = {};

            var parameters = {
                EndPointId: EndPointId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onEndPointUpdated = onEndPointUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/EndPoint/EndPointEditor.html', parameters, settings);
        }

        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            var CarrierAccountTypeArray = UtilsService.getArrayEnum(NP_IVSwitch_CarrierAccountTypeEnum);

 
            drillDownDefinition.title = "EndPoints";
            drillDownDefinition.directive = "np-ivswitch-endpoint-grid";
            drillDownDefinition.hideDrillDownFunction = function (carrierAccountItem) {
                 return (carrierAccountItem.Entity.AccountType == NP_IVSwitch_CarrierAccountTypeEnum.Supplier.value);

             };
            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.ivSwitchEndPointGridAPI = directiveAPI;

                var payload = {
                    CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };
                return carrierAccountItem.ivSwitchEndPointGridAPI.load(payload);
            };
          

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

        }

        function viewHistoryEndPoint(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/EndPoint/EndPointEditor.html', modalParameters, modalSettings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "NP_IVSwitch_EndPoint_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryEndPoint(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "NP_IVSwitch_EndPoint";
        }

        function registerObjectTrackingDrillDownToEndPoint() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, endPointItem) {

                endPointItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: endPointItem.Entity.EndPointId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return endPointItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addEndPoint: addEndPoint,
            editEndPoint: editEndPoint,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToEndPoint: registerObjectTrackingDrillDownToEndPoint,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('NP_IVSwitch_EndPointService', EndPointService);

})(appControllers);