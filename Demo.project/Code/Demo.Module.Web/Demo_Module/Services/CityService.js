app.service('Demo_Module_CityService', ['VRModalService',  'Demo_Module_CityAPIService',
    function (VRModalService, VRNotificationService, UtilsService, Demo_Module_CityAPIService) {
        var drillDownDefinitions = [];
        return ({
            editCity: editCity,
            addCity: addCity,
           
        });
        function editCity(Id, onCityUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCityUpdated = onCityUpdated;
            };
            var parameters = {
                Id: Id
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Views/CityEditor.html', parameters, settings);
        }
        function addCity(onCityAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCityAdded = onCityAdded;
            };
            var parameters = {};
           

            VRModalService.showModal('/Client/Modules/Demo_Module/Views/CityEditor.html', parameters, settings);
        }

        

        function hasNewCityPermission() {
            return Demo_Module_CityAPIService.HasAddCityPermission();
        }

    }]);