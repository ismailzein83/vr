
(function (appControllers) {

    "use strict";

    VRActionService.$inject = ['VRModalService'];

    function VRActionService(VRModalService) {

        function addVRAction(onVRActionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRActionAdded = onVRActionAdded
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRActions/VRActionEditor.html', null, settings);
        };

        function editVRAction(vrActionEntity, onVRActionUpdated) {
            var settings = {};

            var parameters = {
                vrActionEntity: vrActionEntity,
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