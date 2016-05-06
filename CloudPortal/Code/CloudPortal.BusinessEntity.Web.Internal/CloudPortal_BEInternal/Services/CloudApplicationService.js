(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationService.$inject = ['VRModalService'];
    function BEInternal_CloudApplicationService(VRModalService) {

        function addCloudApplication(onCloudApplicationAdded) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCloudApplicationAdded = onCloudApplicationAdded;
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplication/CloudApplicationEditor.html', undefined, modalSettings);
        }

        function editCloudApplication(cloudApplicationId, onCloudApplicationUpdated) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCloudApplicationUpdated = onCloudApplicationUpdated;
            };
            var parameters = {
                CloudApplicationId: cloudApplicationId
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplication/CloudApplicationEditor.html', parameters, modalSettings);
        }

        return ({
            editCloudApplication: editCloudApplication,
            addCloudApplication: addCloudApplication
        });
    }
    appControllers.service('CloudPortal_BEInternal_CloudApplicationService', BEInternal_CloudApplicationService);

})(appControllers);