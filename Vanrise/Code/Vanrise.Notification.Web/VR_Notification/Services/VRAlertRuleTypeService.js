
(function (appControllers) {

    "use strict";

    VRAlertRuleTypeService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRAlertRuleTypeService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addVRAlertRuleType(onVRAlertRuleTypeAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleTypeAdded = onVRAlertRuleTypeAdded
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeEditor.html', null, settings);
        };

        function editVRAlertRuleType(vrAlertRuleTypeId, onVRAlertRuleTypeUpdated) {
            var settings = {};

            var parameters = {
                vrAlertRuleTypeId: vrAlertRuleTypeId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRAlertRuleTypeUpdated = onVRAlertRuleTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeEditor.html', parameters, settings);
        }

        function getEntityUniqueName() {
            return "VR_Notification_AlertRuleType";
        }

        function registerObjectTrackingDrillDownToVRAlertRuleType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrAlertRuleTypeItem) {
                vrAlertRuleTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrAlertRuleTypeItem.Entity.VRAlertRuleTypeId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return vrAlertRuleTypeItem.objectTrackingGridAPI.load(query);
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
            addVRAlertRuleType: addVRAlertRuleType,
            editVRAlertRuleType: editVRAlertRuleType,
            registerObjectTrackingDrillDownToVRAlertRuleType: registerObjectTrackingDrillDownToVRAlertRuleType,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Notification_VRAlertRuleTypeService', VRAlertRuleTypeService);

})(appControllers);