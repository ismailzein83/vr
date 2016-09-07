
(function (appControllers) {

    "use strict";

    VRAlertRuleService.$inject = ['VRModalService'];

    function VRAlertRuleService(VRModalService) {

        function addVRAlertRule(onVRAlertRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleAdded = onVRAlertRuleAdded
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', null, settings);
        };

        function editVRAlertRule(vrAlertRuleId, onVRAlertRuleUpdated) {
            var settings = {};

            var parameters = {
                vrAlertRuleId: vrAlertRuleId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleUpdated = onVRAlertRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', parameters, settings);
        }


        return {
            addVRAlertRule: addVRAlertRule,
            editVRAlertRule: editVRAlertRule
        };
    }

    appControllers.service('VR_Notification_VRAlertRuleService', VRAlertRuleService);

})(appControllers);