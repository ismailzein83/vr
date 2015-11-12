
app.service('VRCommon_CityService', ['VRModalService', 'VRNotificationService', 'UtilsService','VRCommon_CountryService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService) {
        var drillDownDefinitions = [];
        return ({
            editCity: editCity,
            addCity: addCity,
            registerDrillDownToCountry: registerDrillDownToCountry
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

        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Cities";
            drillDownDefinition.directive = "vr-common-city-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New City",
                clicked: function (countryItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(countryItem);
                    var query = {
                        CountriesIds: [countryItem.Entity.CountryId]
                    }
                    var onCityAdded = function (cityObj) {
                        if (countryItem.cityGridAPI != undefined) {
                            countryItem.cityGridAPI.onCityAdded(cityObj);
                        }
                    };
                    addCity(onCityAdded, countryItem.Entity.CountryId);
                }
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
      

    }]);
