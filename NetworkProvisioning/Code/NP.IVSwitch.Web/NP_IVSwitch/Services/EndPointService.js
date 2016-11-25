
(function (appControllers) {

    "use strict";

    EndPointService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService', 'NP_IVSwitch_CarrierAccountTypeEnum','UtilsService'];

    function EndPointService(NPModalService, WhS_BE_CarrierAccountService, NP_IVSwitch_CarrierAccountTypeEnum, UtilsService) {

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
                //     hideCustomerColumn: true
                //  };
                return carrierAccountItem.ivSwitchEndPointGridAPI.load(payload);
            };
            drillDownDefinition.parentMenuActions = [{
                name: 'Add  EndPoint',
                clicked: function (carrierAccountItem) {
                    //if (EndPointTab.setTabSelected != undefined)
                    //    EndPointTab.setTabSelected(parentAccount);
                    var onEndPointAdded = function (addedEndPoint) {
                        if (carrierAccountItem.ivSwitchEndPointGridAPI != undefined)
                            carrierAccountItem.ivSwitchEndPointGridAPI.onEndPointAdded(addedEndPoint);
                    };
                    addEndPoint(carrierAccountItem.Entity.CarrierAccountId, onEndPointAdded);
                },
            }];

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

        }
        return {
            addEndPoint: addEndPoint,
            editEndPoint: editEndPoint,
           registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount
        };
    }

    appControllers.service('NP_IVSwitch_EndPointService', EndPointService);

})(appControllers);