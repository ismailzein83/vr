app.service('WhS_BE_CarrierAccountService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {


        function addCarrierAccount(onCarrierAccountAdded, dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Carrier Account";
                modalScope.onCarrierAccountAdded = onCarrierAccountAdded;
            };
            var parameters;
            if (dataItem != undefined) {
                parameters = {
                    CarrierProfileId: dataItem.CarrierProfileId,
                };
            }
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, settings);
        }

        function editCarrierAccount(carrierAccountObj, onCarrierAccountUpdated) {
            var modalSettings = {
            };
            var parameters = {
                CarrierAccountId: carrierAccountObj.CarrierAccountId,
                CarrierProfileId: carrierAccountObj.CarrierProfileId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Carrier Account";
                modalScope.onCarrierAccountUpdated = onCarrierAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, modalSettings);
        }


        return ({
            addCarrierAccount: addCarrierAccount,
            editCarrierAccount: editCarrierAccount
        });

    }]);