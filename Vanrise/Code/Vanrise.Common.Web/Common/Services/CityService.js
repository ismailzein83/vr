
app.service('VRCommon_CityService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService', 'VRCommon_CityAPIService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService, VRCommon_CityAPIService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editCity: editCity,
            addCity: addCity,
            registerDrillDownToCountry: registerDrillDownToCountry,
            registerObjectTrackingDrillDownToCity: registerObjectTrackingDrillDownToCity,
            getDrillDownDefinition: getDrillDownDefinition
        });
        function editCity(cityId, onCityUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCityUpdated = onCityUpdated;
            };
            var parameters = {
                CityId: cityId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', parameters, settings);
        }
        function addCity(onCityAdded , countryId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCityAdded = onCityAdded;
            };
            var parameters = {};
            if (countryId != undefined) {
                parameters.CountryId = countryId; 
            }

            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_City";
        }

        function registerObjectTrackingDrillDownToCity() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, cityItem) {

                cityItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: cityItem.Entity.CityId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return cityItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Cities";
            drillDownDefinition.directive = "vr-common-city-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New City",
                clicked: function (countryItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(countryItem);
                   
                    var onCityAdded = function (cityObj) {
                        if (countryItem.cityGridAPI != undefined) {
                            countryItem.cityGridAPI.onCityAdded(cityObj);
                        }
                    };
                    addCity(onCityAdded, countryItem.Entity.CountryId);
                },
                haspermission: hasNewCityPermission
            }];

            drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
                countryItem.cityGridAPI = directiveAPI;
                var query = {
                    CountryIds: [countryItem.Entity.CountryId],
                };
               
                return countryItem.cityGridAPI.loadGrid(query);
            };

            VRCommon_CountryService.addDrillDownDefinition(drillDownDefinition);
        }

        function hasNewCityPermission() {
            return VRCommon_CityAPIService.HasAddCityPermission();
        }

    }]);
