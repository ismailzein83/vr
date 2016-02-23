(function (appControllers) {

    'use stict';

    OperatorProfileService.$inject = ['VRModalService'];

    function OperatorProfileService(VRModalService) {
        return ({
            addOperatorProfile: addOperatorProfile,
            editOperatorProfile: editOperatorProfile
        });

        function addOperatorProfile(onOperatorProfileAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onOperatorProfileAdded = onOperatorProfileAdded;
            };

            VRModalService.showModal('/Client/Modules/InterConnect_BusinessEntity/Views/OperatorProfile/OperatorProfileEditor.html', null, settings);
        }

        function editOperatorProfile(OperatorProfileObj, onOperatorProfileUpdated) {
            var modalSettings = {
            };

            var parameters = {
                OperatorProfileId: OperatorProfileObj.Entity.OperatorProfileId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorProfileUpdated = onOperatorProfileUpdated;
            };
            VRModalService.showModal('/Client/Modules/InterConnect_BusinessEntity/Views/OperatorProfile/OperatorProfileEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('InterConnect_BE_OperatorProfileService', OperatorProfileService);

})(appControllers);
