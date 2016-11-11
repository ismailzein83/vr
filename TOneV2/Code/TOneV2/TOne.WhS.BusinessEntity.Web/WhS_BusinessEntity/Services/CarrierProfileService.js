(function (appControllers) {

    'use strict';

    CarrierProfileService.$inject = ['VRModalService'];

    function CarrierProfileService(VRModalService) {
        return ({
            addCarrierProfile: addCarrierProfile,
            editCarrierProfile: editCarrierProfile
        });

        function addCarrierProfile(onCarrierProfileAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onCarrierProfileAdded = onCarrierProfileAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', null, settings);
        }

        function editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated) {
            var modalSettings = {
            };

            var parameters = {
                CarrierProfileId: carrierProfileObj.Entity.CarrierProfileId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCarrierProfileUpdated = onCarrierProfileUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('WhS_BE_CarrierProfileService', CarrierProfileService);

})(appControllers);
