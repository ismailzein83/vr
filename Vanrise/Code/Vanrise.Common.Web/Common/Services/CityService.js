
app.service('VRCommon_CityService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            editCity: editCity,
            addCity: addCity,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition

        });
        function editCity(obj, onCityUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Entity.Name, "City");
                modalScope.onCityUpdated = onCityUpdated;
            };
            var parameters = {
                CityId: obj.Entity.CityId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', parameters, settings);
        }
        function addCity(onCityAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("City");
                modalScope.onCityAdded = onCityAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', parameters, settings);
        }

        

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
