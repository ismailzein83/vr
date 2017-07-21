
(function (appControllers) {

    "use strict";

    RouteService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService', 'NP_IVSwitch_CarrierAccountTypeEnum', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteService(NPModalService, WhS_BE_CarrierAccountService, NP_IVSwitch_CarrierAccountTypeEnum, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addRoute(CarrierAccountId, onRouteAdded) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

             settings.onScopeReady = function (modalScope) {
                modalScope.onRouteAdded = onRouteAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Route/RouteEditor.html', parameters, settings);
        };
        function editRoute(RouteId, CarrierAccountId, onRouteUpdated) {
            var settings = {};
                      

            var parameters = {
                RouteId: RouteId,
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteUpdated = onRouteUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Route/RouteEditor.html', parameters, settings);
        }

        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            var CarrierAccountTypeArray = UtilsService.getArrayEnum(NP_IVSwitch_CarrierAccountTypeEnum);


            drillDownDefinition.title = "Routes";
            drillDownDefinition.directive = "np-ivswitch-route-grid";
            drillDownDefinition.hideDrillDownFunction = function (carrierAccountItem) {
                  return ( carrierAccountItem.Entity.AccountType == NP_IVSwitch_CarrierAccountTypeEnum.Customer.value);

            };
            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.ivSwitchRouteGridAPI = directiveAPI;

                var payload = {
                    CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };
                return carrierAccountItem.ivSwitchRouteGridAPI.load(payload);
            };           
            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

        }

        function viewHistoryRoute(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Route/RouteEditor.html', modalParameters, modalSettings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "NP_IVSwitch_Route_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryRoute(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "NP_IVSwitch_Route";
        }

        function registerObjectTrackingDrillDownToRoute() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, routeItem) {

                routeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: routeItem.Entity.RouteId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return routeItem.objectTrackingGridAPI.load(query);
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
            addRoute: addRoute,
            editRoute: editRoute,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            registerObjectTrackingDrillDownToRoute: registerObjectTrackingDrillDownToRoute,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction

        };
    }

    appControllers.service('NP_IVSwitch_RouteService', RouteService);

})(appControllers);