(function (appControllers) {
    'use stict';
    CityService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService', 'VRCommon_CityAPIService', 'VRCommon_ObjectTrackingService', 'VRCommon_RegionService'];
    function CityService(VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService, VRCommon_CityAPIService, VRCommon_ObjectTrackingService, VRCommon_RegionService) {
        var drillDownDefinitions = [];

        function viewHistoryCity(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', modalParameters, modalSettings);
        };

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

        function addCity(onCityAdded, countryId, regionId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCityAdded = onCityAdded;
            };
            var parameters = {};
            if (countryId != undefined) {
                parameters.CountryId = countryId;
            }

            if (countryId != undefined) {
                parameters.RegionId = regionId;
            }

            VRModalService.showModal('/Client/Modules/Common/Views/City/CityEditor.html', parameters, settings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_City_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryCity(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function getEntityUniqueName() {
            return "VR_Common_City";
        }

        function registerObjectTrackingDrillDownToCity() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.localizedtitle = VRCommon_ObjectTrackingService.getObjectTrackingGridLocalizedTitle();
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

        function registerDrillDownToRegion() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Cities";
            drillDownDefinition.localizedtitle = "VRRes.Cities.VREnd";

            drillDownDefinition.directive = "vr-common-city-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New City",
                clicked: function (regionItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(regionItem);

                    var onCityAdded = function (cityObj) {
                        if (regionItem.cityGridAPI != undefined) {
                            regionItem.cityGridAPI.onCityAdded(cityObj);
                        }
                    };
                    addCity(onCityAdded, regionItem.Entity.CountryId, regionItem.Entity.RegionId);
                },
                haspermission: hasNewCityPermission
            }];

            drillDownDefinition.loadDirective = function (directiveAPI, regionItem) {
                regionItem.cityGridAPI = directiveAPI;
                var query = {
                    CountryIds: [regionItem.Entity.CountryId],
                    RegionIds: [regionItem.Entity.RegionId]
                };

                return regionItem.cityGridAPI.loadGrid(query);
            };

            VRCommon_RegionService.addDrillDownDefinition(drillDownDefinition);
        }

        function hasNewCityPermission() {
            return VRCommon_CityAPIService.HasAddCityPermission();
        }

        return {
            editCity: editCity,
            addCity: addCity,
            registerDrillDownToCountry: registerDrillDownToCountry,
            registerDrillDownToRegion: registerDrillDownToRegion,
            registerObjectTrackingDrillDownToCity: registerObjectTrackingDrillDownToCity,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('VRCommon_CityService', CityService);

})(appControllers);
