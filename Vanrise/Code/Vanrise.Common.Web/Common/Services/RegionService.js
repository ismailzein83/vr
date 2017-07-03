
app.service('VRCommon_RegionService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService', 'VRCommon_RegionAPIService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService, VRCommon_RegionAPIService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editRegion: editRegion,
            addRegion: addRegion,
            registerDrillDownToCountry: registerDrillDownToCountry,
            registerObjectTrackingDrillDownToRegion: registerObjectTrackingDrillDownToRegion,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction,
            addDrillDownDefinition: addDrillDownDefinition
        });

        function viewHistoryRegion(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/Region/RegionEditor.html', modalParameters, modalSettings);
        };

        function editRegion(regionId, onRegionUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRegionUpdated = onRegionUpdated;
            };
            var parameters = {
                RegionId: regionId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Region/RegionEditor.html', parameters, settings);
        }
        function addRegion(onRegionAdded , countryId,disableCountry) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRegionAdded = onRegionAdded;
            };
            var parameters = {};
            if (countryId != undefined) {
                parameters.CountryId = countryId;
             
            }
            if (disableCountry != undefined)
                parameters.disableCountry = disableCountry;

            VRModalService.showModal('/Client/Modules/Common/Views/Region/RegionEditor.html', parameters, settings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_Region_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryRegion(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "VR_Common_Region";
        }

        function registerObjectTrackingDrillDownToRegion() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, regionItem) {

                regionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: regionItem.Entity.RegionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return regionItem.objectTrackingGridAPI.load(query);
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

            drillDownDefinition.title = "Regions";
            drillDownDefinition.directive = "vr-common-region-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New Region",
                clicked: function (countryItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(countryItem);
                   
                    var onRegionAdded = function (regionObj) {
                        if (countryItem.regionGridAPI != undefined) {
                            countryItem.regionGridAPI.onRegionAdded(regionObj);
                        }
                    };
                    addRegion(onRegionAdded, countryItem.Entity.CountryId);
                },
                haspermission: hasNewRegionPermission
            }];

            drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
                countryItem.regionGridAPI = directiveAPI;
                var query = {
                    CountryIds: [countryItem.Entity.CountryId],
                };
               
                return countryItem.regionGridAPI.loadGrid(query);
            };

            VRCommon_CountryService.addDrillDownDefinition(drillDownDefinition);
        }

        function hasNewRegionPermission() {
            return VRCommon_RegionAPIService.HasAddRegionPermission();
        }

    }]);
