(function (appControllers) {

    "use strict";

    VRAlertRuleService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function VRAlertRuleService(VRModalService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addVRAlertRule(onVRAlertRuleAdded, context) {

            var parameters = {
                context: context,
                isViewMode: false
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleAdded = onVRAlertRuleAdded
            };

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', parameters, settings);
        };

        function editVRAlertRule(vrAlertRuleId, onVRAlertRuleUpdated, context) {

            var parameters = {
                vrAlertRuleId: vrAlertRuleId,
                context: context,
                isViewMode: false
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleUpdated = onVRAlertRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', parameters, settings);
        }

        function viewVRAlertRule(vrAlertRuleId, context) {

            var parameters = {
                vrAlertRuleId: vrAlertRuleId,
                context: context,
                isViewMode: true
            };

            var settings = {};

            //settings.onScopeReady = function (modalScope) {
            //    UtilsService.setContextReadOnly(modalScope);
            //};

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRule/VRAlertRuleEditor.html', parameters, settings);
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
        function getEntityUniqueName(ruleTypeId) {
            return "VR_Notification_AlertRule_" + ruleTypeId;
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
            viewVRAlertRule: viewVRAlertRule,
            registerObjectTrackingDrillDownToAlertRule: registerObjectTrackingDrillDownToAlertRule,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Notification_VRAlertRuleService', VRAlertRuleService);

})(appControllers);
