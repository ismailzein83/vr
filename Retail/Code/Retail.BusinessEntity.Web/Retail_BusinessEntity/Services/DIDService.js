(function (appControllers) {

    'use stict';

    DIDService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function DIDService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addDID(onDIDAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDIDAdded = onDIDAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/DID/DIDEditor.html', null, settings);
        };
        function editDID(dIDId, onDIDUpdated) {

            var parameters = {
               dIDId: dIDId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDIDUpdated = onDIDUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/DID/DIDEditor.html', parameters, settings);
        }

        function registerObjectTrackingDrillDownToDID() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();

            drillDownDefinition.directive = "vr-common-objecttracking-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, didItem) {
                didItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: didItem.Entity.DIDId,
                    EntityUniqueName: getEntityUniqueName(),
                };
                return didItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }
        function getEntityUniqueName() {
            return "Retail_BusinessEntity_DID";
        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }
        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addDID: addDID,
            editDID: editDID,
            registerObjectTrackingDrillDownToDID: registerObjectTrackingDrillDownToDID,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('Retail_BE_DIDService', DIDService);

})(appControllers);