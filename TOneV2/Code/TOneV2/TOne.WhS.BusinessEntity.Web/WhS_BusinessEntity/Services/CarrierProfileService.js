
app.service('WhS_BE_CarrierProfileService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {


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


        return ({
            addCarrierProfile: addCarrierProfile,
            editCarrierProfile: editCarrierProfile
        });

    }]);