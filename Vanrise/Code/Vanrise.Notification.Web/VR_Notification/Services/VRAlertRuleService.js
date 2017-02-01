
(function (appControllers) {

    "use strict";

    VRAlertRuleService.$inject = ['VRModalService'];

    function VRAlertRuleService(VRModalService) {

        function addVRAlertRule(onVRAlertRuleAdded, context) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleAdded = onVRAlertRuleAdded
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', parameters, settings);
        };

        function editVRAlertRule(vrAlertRuleId, onVRAlertRuleUpdated, context) {
            var settings = {};

            var parameters = {
                vrAlertRuleId: vrAlertRuleId,
                context: context
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