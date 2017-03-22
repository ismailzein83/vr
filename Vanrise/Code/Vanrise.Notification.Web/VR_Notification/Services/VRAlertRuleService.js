
(function (appControllers) {

    "use strict";

    VRAlertRuleService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRAlertRuleService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
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
        function getEntityUniqueName(ruleTypeId) {
            return "VR_Notification_AlertRule_" + ruleTypeId;
        }

        function registerObjectTrackingDrillDownToAlertRule() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, alertRuleItem) {
                alertRuleItem.objectTrackingGridAPI = directiveAPI;
                
                var query = {
                    ObjectId: alertRuleItem.Entity.VRAlertRuleId,
                    EntityUniqueName: getEntityUniqueName(alertRuleItem.Entity.RuleTypeId),

                };
                return alertRuleItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addVRAlertRule: addVRAlertRule,
            editVRAlertRule: editVRAlertRule,
            registerObjectTrackingDrillDownToAlertRule: registerObjectTrackingDrillDownToAlertRule,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Notification_VRAlertRuleService', VRAlertRuleService);

})(appControllers);
