
(function (appControllers) {

    "use strict";

    VRMailMessageTemplateService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRMailMessageTemplateService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addMailMessageTemplate(onMailMessageTemplateAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTemplateAdded = onMailMessageTemplateAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTemplateEditor.html', null, settings);
        };

        function editMailMessageTemplate(mailMessageTemplateId, onMailMessageTemplateUpdated) {
            var settings = {};

            var parameters = {
                mailMessageTemplateId: mailMessageTemplateId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTemplateUpdated = onMailMessageTemplateUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTemplateEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_VRMailMessageTemplate";
        }

        function registerObjectTrackingDrillDownToVRMailTemplateMessage() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, mailItem) {
                mailItem.objectTrackingGridAPI = directiveAPI;
                
                var query = {
                    ObjectId: mailItem.Entity.VRMailMessageTemplateId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return mailItem.objectTrackingGridAPI.load(query);
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
            addMailMessageTemplate: addMailMessageTemplate,
            editMailMessageTemplate: editMailMessageTemplate,
            registerObjectTrackingDrillDownToVRMailTemplateMessage: registerObjectTrackingDrillDownToVRMailTemplateMessage,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRMailMessageTemplateService', VRMailMessageTemplateService);

})(appControllers);