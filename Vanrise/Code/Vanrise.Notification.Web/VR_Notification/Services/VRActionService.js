
(function (appControllers) {

    "use strict";

    VRActionService.$inject = ['VRModalService'];

    function VRActionService(VRModalService) {

        function addVRAction(onVRActionAdded, extensionType,context) {
            var settings = {};

            var parameters = {
                extensionType: extensionType,
                context: context
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRActionAdded = onVRActionAdded
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRActions/VRActionEditor.html', parameters, settings);
        };

        function editVRAction(vrActionEntity, onVRActionUpdated, extensionType, context) {
            var settings = {};

            var parameters = {
                vrActionEntity: vrActionEntity,
                extensionType: extensionType,
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRActionUpdated = onVRActionUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRActions/VRActionEditor.html', parameters, settings);
        }


        return {
            addVRAction: addVRAction,
            editVRAction: editVRAction
        };
    }

    appControllers.service('VR_Notification_VRActionService', VRActionService);

})(appControllers);