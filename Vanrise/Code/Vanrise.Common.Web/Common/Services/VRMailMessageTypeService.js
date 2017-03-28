
(function (appControllers) {

    "use strict";

    VRMailMessageTypeService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRMailMessageTypeService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addMailMessageType(onMailMessageTypeAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTypeAdded = onMailMessageTypeAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTypeEditor.html', null, settings);
        };

        function editMailMessageType(mailMessageTypeId, onMailMessageTypeUpdated) {
            var settings = {};

            var parameters = {
                mailMessageTypeId: mailMessageTypeId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTypeUpdated = onMailMessageTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTypeEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_MailMessageType";
        }

        function registerObjectTrackingDrillDownToVRMailMessageMailType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrMailMessageTypeItem) {
               
                vrMailMessageTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrMailMessageTypeItem.Entity.VRMailMessageTypeId,
                    EntityUniqueName: getEntityUniqueName(),

                };

                return vrMailMessageTypeItem.objectTrackingGridAPI.load(query);
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
            addMailMessageType: addMailMessageType,
            editMailMessageType: editMailMessageType,
            registerObjectTrackingDrillDownToVRMailMessageMailType: registerObjectTrackingDrillDownToVRMailMessageMailType,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRMailMessageTypeService', VRMailMessageTypeService);

})(appControllers);