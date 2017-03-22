
app.service('VRCommon_VRTimeZoneService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editVRTimeZone: editVRTimeZone,
            addVRTimeZone: addVRTimeZone,
            registerObjectTrackingDrillDownToTimeZone: registerObjectTrackingDrillDownToTimeZone,
            getDrillDownDefinition: getDrillDownDefinition

        });
        function editVRTimeZone(vrTimeZoneId, onVRTimeZoneUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRTimeZoneUpdated = onVRTimeZoneUpdated;
            };
            var parameters = {
                VRTimeZoneId: vrTimeZoneId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRTimeZone/VRTimeZoneEditor.html', parameters, settings);
        }
        function addVRTimeZone(onVRTimeZoneAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRTimeZoneAdded = onVRTimeZoneAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/VRTimeZone/VRTimeZoneEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_TimeZone";
        }

        function registerObjectTrackingDrillDownToTimeZone() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, timeZoneItem) {
                timeZoneItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: timeZoneItem.Entity.TimeZoneId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return timeZoneItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
