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

            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorAccount/OperatorProfileEditor.html', null, settings);
        }

        function editOperatorProfile(operatorProfileObj, onOperatorProfileUpdated) {
            var modalSettings = {
            };

            var parameters = {
                OperatorProfileId: operatorProfileObj.Entity.OperatorProfileId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorProfileUpdated = onOperatorProfileUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorAccount/OperatorProfileEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_OperatorProfileService', OperatorProfileService);

})(appControllers);
