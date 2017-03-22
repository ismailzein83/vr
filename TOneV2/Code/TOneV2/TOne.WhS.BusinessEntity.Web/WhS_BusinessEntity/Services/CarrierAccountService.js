(function (appControllers) {

    'use strict';

    CarrierAccountService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function CarrierAccountService(VRModalService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addCarrierAccount: addCarrierAccount,
            editCarrierAccount: editCarrierAccount,
            viewCarrierAccount: viewCarrierAccount,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToCarrierAccount: registerObjectTrackingDrillDownToCarrierAccount
        });

        function addCarrierAccount(onCarrierAccountAdded, dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCarrierAccountAdded = onCarrierAccountAdded;
            };
            var parameters;
            if (dataItem != undefined) {
                parameters = {
                    CarrierProfileId: dataItem.CarrierProfileId
                };
            }
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, settings);
        }

        function editCarrierAccount(carrierAccountObj, onCarrierAccountUpdated) {
            var modalSettings = {
            };
            var parameters = {
                CarrierAccountId: carrierAccountObj.CarrierAccountId != undefined?carrierAccountObj.CarrierAccountId:carrierAccountObj,
                CarrierProfileId: carrierAccountObj.CarrierProfileId != undefined? carrierAccountObj.CarrierProfileId:undefined
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCarrierAccountUpdated = onCarrierAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, modalSettings);
        }

        function viewCarrierAccount(CarrierAccountId) {
            var modalSettings = {
            };
            var parameters = {
                CarrierAccountId: CarrierAccountId
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope)
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "WhS_BusinessEntity_CarrierAccount";
        }

        function registerObjectTrackingDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: carrierAccountItem.Entity.CarrierAccountId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return carrierAccountItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('WhS_BE_CarrierAccountService', CarrierAccountService);

})(appControllers);
