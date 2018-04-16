
(function (appControllers) {

    "use strict";

    SMSMessageTemplateService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function SMSMessageTemplateService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addSMSMessageTemplate(onSMSMessageTemplateAdded, smsMessageTypeId) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSMSMessageTemplateAdded = onSMSMessageTemplateAdded;
            };

            var parameters = {
                smsMessageTypeId: smsMessageTypeId,
            };

            VRModalService.showModal('/Client/Modules/Common/Views/SMS/SMSMessageTemplateEditor.html', parameters, settings);
        }

        function editSMSMessageTemplate(smsMessageTemplateId, onSMSMessageTemplateUpdated) {
            var settings = {};

            var parameters = {
                smsMessageTemplateId: smsMessageTemplateId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSMSMessageTemplateUpdated = onSMSMessageTemplateUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/SMS/SMSMessageTemplateEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_SMSMessageTemplate";
        }

        function registerObjectTrackingDrillDownToSMSTemplateMessage() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, smsItem) {
                smsItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: smsItem.Entity.SMSMessageTemplateId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return smsItem.objectTrackingGridAPI.load(query);
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
            addSMSMessageTemplate: addSMSMessageTemplate,
            editSMSMessageTemplate: editSMSMessageTemplate,
            registerObjectTrackingDrillDownToSMSTemplateMessage: registerObjectTrackingDrillDownToSMSTemplateMessage,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_SMSMessageTemplateService', SMSMessageTemplateService);

})(appControllers);