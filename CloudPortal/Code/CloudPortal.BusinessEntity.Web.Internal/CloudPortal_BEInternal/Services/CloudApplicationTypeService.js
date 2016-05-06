(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationTypeService.$inject = ['VRModalService'];
    function BEInternal_CloudApplicationTypeService(VRModalService) {

        function addCloudApplicationType(onCloudApplicationTypeAdded) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCloudApplicationTypeAdded = onCloudApplicationTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplicationType/CloudApplicationTypeEditor.html', undefined, modalSettings);
        }

        function editCloudApplicationType(cloudApplicationTypeId, onCloudApplicationTypeUpdated) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCloudApplicationTypeUpdated = onCloudApplicationTypeUpdated;
            };
            var parameters = {
                CloudApplicationTypeId: cloudApplicationTypeId
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplicationType/CloudApplicationTypeEditor.html', parameters, modalSettings);
        }

        return ({
            editCloudApplicationType: editCloudApplicationType,
            addCloudApplicationType: addCloudApplicationType
        });
    }
    appControllers.service('CloudPortal_BEInternal_CloudApplicationTypeService', BEInternal_CloudApplicationTypeService);

})(appControllers);