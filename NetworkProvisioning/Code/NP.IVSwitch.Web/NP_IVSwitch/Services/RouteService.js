
(function (appControllers) {

    "use strict";

    RouteService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService', 'NP_IVSwitch_CarrierAccountTypeEnum', 'UtilsService'];

    function RouteService(NPModalService, WhS_BE_CarrierAccountService, NP_IVSwitch_CarrierAccountTypeEnum, UtilsService) {

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
                //     hideCustomerColumn: true
                //  };
                return carrierAccountItem.ivSwitchRouteGridAPI.load(payload);
            };
            drillDownDefinition.parentMenuActions = [{
                name: 'Add  Route',
                clicked: function (carrierAccountItem) {
                    //if (EndPointTab.setTabSelected != undefined)
                    //    EndPointTab.setTabSelected(parentAccount);
                    var onRouteAdded = function (addedRoute) {
                         if (carrierAccountItem.ivSwitchRouteGridAPI != undefined)
                            carrierAccountItem.ivSwitchRouteGridAPI.onRouteAdded(addedRoute);
                    };
                    addRoute(carrierAccountItem.Entity.CarrierAccountId, onRouteAdded);
                },
            }];

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

        }
        return {
            addRoute: addRoute,
            editRoute: editRoute,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,

        };
    }

    appControllers.service('NP_IVSwitch_RouteService', RouteService);

})(appControllers);