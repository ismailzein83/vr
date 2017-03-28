
(function (appControllers) {

    'use stict';

    AlertLEvelService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function AlertLEvelService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addAlertLevel(onAlertLevelAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelAdded = onAlertLevelAdded
            };

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertLevel/VRAlertLevelEditor.html', null, settings);
        }

        function editAlertLevel(alertLevelId, onAlertLevelUpdated) {
            var settings = {};

            var parameters = {
                alertLevelId: alertLevelId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelUpdated = onAlertLevelUpdated;
            };
            
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertLevel/VRAlertLevelEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Notification_AlertLevel";
        }

        function registerObjectTrackingDrillDownToVRAlertLevel() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vralertLevelItem) {
                vralertLevelItem.objectTrackingGridAPI = directiveAPI;
               
                var query = {
                    ObjectId: vralertLevelItem.Entity.VRAlertLevelId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return vralertLevelItem.objectTrackingGridAPI.load(query);
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
            addAlertLevel: addAlertLevel,
            editAlertLevel: editAlertLevel,
            registerObjectTrackingDrillDownToVRAlertLevel: registerObjectTrackingDrillDownToVRAlertLevel,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Notification_AlertLevelService', AlertLEvelService);

})(appControllers);