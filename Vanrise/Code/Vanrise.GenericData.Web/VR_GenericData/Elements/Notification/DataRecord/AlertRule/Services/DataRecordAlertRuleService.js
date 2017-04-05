
(function (appControllers) {

    "use strict";

    DataRecordAlertRuleService.$inject = ['VRModalService'];

    function DataRecordAlertRuleService(VRModalService) {

        function addDataRecordAlertRule(context, onDataRecordAlertRuleAdded) {

            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordAlertRuleAdded = onDataRecordAlertRuleAdded
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Views/AlertRuleRecordEditor.html', parameters, settings);
        }

        function editDataRecordAlertRule(dataRecordAlertRuleEntity, context, onDataRecordAlertRuleUpdated) {

            var parameters = {
                dataRecordAlertRuleEntity: dataRecordAlertRuleEntity,
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordAlertRuleUpdated = onDataRecordAlertRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Views/AlertRuleRecordEditor.html', parameters, settings);
        }


        return {
            addDataRecordAlertRule: addDataRecordAlertRule,
            editDataRecordAlertRule: editDataRecordAlertRule
        };
    }
    appControllers.service('VR_GenericData_DataRecordAlertRuleService', DataRecordAlertRuleService);

})(appControllers);