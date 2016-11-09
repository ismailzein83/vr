
(function (appControllers) {

    "use strict";

    VRComponentTypeService.$inject = ['VRModalService'];

    function VRComponentTypeService(VRModalService) {

        function addVRComponentType(extensionConfigId, onVRComponentTypeAdded) {
            var settings = {};
            var parameters = {
                extensionConfigId: extensionConfigId,
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRComponentTypeAdded = onVRComponentTypeAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRComponentType/VRComponentTypeEditor.html', parameters, settings);
        };

        function editVRComponentType(extensionConfigId, vrComponentTypeId, onVRComponentTypeUpdated) {
            var settings = {};

            var parameters = {
                vrComponentTypeId: vrComponentTypeId,
                extensionConfigId: extensionConfigId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRComponentTypeUpdated = onVRComponentTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRComponentType/VRComponentTypeEditor.html', parameters, settings);
        }


        return {
            addVRComponentType: addVRComponentType,
            editVRComponentType: editVRComponentType
        };
    }

    appControllers.service('VRCommon_VRComponentTypeService', VRComponentTypeService);

})(appControllers);