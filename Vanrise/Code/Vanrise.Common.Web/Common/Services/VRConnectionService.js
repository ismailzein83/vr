
(function (appControllers) {

    "use strict";

    VRConnectionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function VRConnectionService(VRModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
        function addVRConnection(onVRConnectionAdded, connectionTypeIds) {
            var settings = {};
            
            var parameters = {
                connectionTypeIds: connectionTypeIds
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRConnectionAdded = onVRConnectionAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRConnection/VRConnectionEditor.html', parameters, settings);
        }

        function editVRConnection(vrConnectionId, onVRConnectionUpdated) {
            var settings = {};

            var parameters = {
                vrConnectionId: vrConnectionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRConnectionUpdated = onVRConnectionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRConnection/VRConnectionEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_Connection";
        }
        function registerObjectTrackingDrillDownToConnection() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, connectionItem) {

                connectionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: connectionItem.Entity.VRConnectionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return connectionItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function viewVRConnection(vrConnectionId, viewMode) {

            var parameters = {
                vrConnectionId: vrConnectionId,
                isReadOnly: true
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                if (viewMode != undefined && viewMode == true) {
                    UtilsService.setContextReadOnly(modalScope);
                    modalScope.viewMode = true;
                }
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRConnection/VRConnectionEditor.html', parameters, settings);
        }

        return {
            addVRConnection: addVRConnection,
            editVRConnection: editVRConnection,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToConnection: registerObjectTrackingDrillDownToConnection,
            viewVRConnection: viewVRConnection
        };
    }

    appControllers.service('VRCommon_VRConnectionService', VRConnectionService);

})(appControllers);