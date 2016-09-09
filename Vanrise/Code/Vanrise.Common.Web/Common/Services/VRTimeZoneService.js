
app.service('VRCommon_VRTimeZoneService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            editVRTimeZone: editVRTimeZone,
            addVRTimeZone: addVRTimeZone

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

    }]);
