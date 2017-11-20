
(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceService.$inject = ['VRModalService'];

    function VRLocalizationTextResourceService(VRModalService) {

        function addVRLocalizationTextResource(onVRLocalizationTextResourceAdded) {
            var settings = {};
            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceAdded = onVRLocalizationTextResourceAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceEditor.html', parameters, settings);
        }

        function editVRLocalizationTextResource(vrLocalizationTextResourceId, onVRLocalizationTextResourceUpdated) {
            var settings = {};
            var parameters = {
                vrLocalizationTextResourceId: vrLocalizationTextResourceId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceUpdated = onVRLocalizationTextResourceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceEditor.html', parameters, settings);
        }

        return {
            addVRLocalizationTextResource: addVRLocalizationTextResource,
            editVRLocalizationTextResource: editVRLocalizationTextResource
        };
    }

    appControllers.service('VRCommon_VRLocalizationTextResourceService', VRLocalizationTextResourceService);

})(appControllers);