
(function (appControllers) {

    "use strict";

    VRAlertRuleTypeService.$inject = ['VRModalService'];

    function VRAlertRuleTypeService(VRModalService) {

        function addVRAlertRuleType(onVRAlertRuleTypeAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleTypeAdded = onVRAlertRuleTypeAdded
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeEditor.html', null, settings);
        };

        function editVRAlertRuleType(styleDefinitionId, onVRAlertRuleTypeUpdated) {
            var settings = {};

            var parameters = {
                styleDefinitionId: styleDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleTypeUpdated = onVRAlertRuleTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeEditor.html', parameters, settings);
        }


        return {
            addVRAlertRuleType: addVRAlertRuleType,
            editVRAlertRuleType: editVRAlertRuleType
        };
    }

    appControllers.service('VR_Notification_VRAlertRuleTypeService', VRAlertRuleTypeService);

})(appControllers);