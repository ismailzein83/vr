(function (appControllers) {

    'use strict';

    CarrierAccountService.$inject = ['VRModalService', 'UtilsService'];

    function CarrierAccountService(VRModalService, UtilsService) {
        return ({
            addCarrierAccount: addCarrierAccount,
            editCarrierAccount: editCarrierAccount,
            viewCarrierAccount: viewCarrierAccount
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
    }

    appControllers.service('WhS_BE_CarrierAccountService', CarrierAccountService);

})(appControllers);
