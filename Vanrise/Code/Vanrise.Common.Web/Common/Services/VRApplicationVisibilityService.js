(function (appControllers) {

    "use strict";

    VRApplicationVisibilityService.$inject = ['VRModalService'];

    function VRApplicationVisibilityService(VRModalService) {

        function addVRApplicationVisibility(onVRApplicationVisibilityAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRApplicationVisibilityAdded = onVRApplicationVisibilityAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRApplicationVisibility/VRApplicationVisibilityEditor.html', null, settings);
        };
        function editVRApplicationVisibility(vrApplicationVisibilityId, onVRApplicationVisibilityUpdated) {
            var settings = {};

            var parameters = {
                vrApplicationVisibilityId: vrApplicationVisibilityId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRApplicationVisibilityUpdated = onVRApplicationVisibilityUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRApplicationVisibility/VRApplicationVisibilityEditor.html', parameters, settings);
        }

        function addVRModuleVisibility(onVRModuleVisibilityAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRModuleVisibilityAdded = onVRModuleVisibilityAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/VRApplicationVisibility/Templates/VRModuleVisibilityEditor.html', null, settings);
        };
        function editVRModuleVisibility(vrModuleVisibility, onVRModuleVisibilityUpdated) {
            var settings = {};

            var parameters = {
                vrModuleVisibility: vrModuleVisibility
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRModuleVisibilityUpdated = onVRModuleVisibilityUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/VRApplicationVisibility/Templates/VRModuleVisibilityEditor.html', parameters, settings);
        }


        return {
            addVRApplicationVisibility: addVRApplicationVisibility,
            editVRApplicationVisibility: editVRApplicationVisibility,
            addVRModuleVisibility: addVRModuleVisibility,
            editVRModuleVisibility: editVRModuleVisibility
        };
    }

    appControllers.service('VRCommon_VRApplicationVisibilityService', VRApplicationVisibilityService);

})(appControllers);