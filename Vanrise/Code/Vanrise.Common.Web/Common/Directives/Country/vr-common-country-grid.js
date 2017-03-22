"use strict";

app.directive("vrCommonCountryGrid", ["UtilsService", "VRNotificationService", "VRCommon_CountryAPIService", "VRCommon_CountryService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VRCommon_CountryAPIService, VRCommon_CountryService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var countryGrid = new CountryGrid($scope, ctrl, $attrs);
            countryGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Country/Templates/CountryGridTemplate.html"

    };

    function CountryGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        var DrillDownDefinitionsArray = [];
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.countries = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
               
                var drillDownDefinitions = VRCommon_CountryService.getDrillDownDefinition();
                registerObjectTrackingDrillDownToCountry();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(DrillDownDefinitionsArray, gridAPI, $scope.gridMenuActions);

                function registerObjectTrackingDrillDownToCountry() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                    drillDownDefinition.directive = "vr-common-objecttracking-grid";


                    drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {

                        countryItem.objectTrackingGridAPI = directiveAPI;
                        var query = {
                            ObjectId: countryItem.Entity.CountryId,
                            EntityUniqueName: VRCommon_CountryService.getEntityUniqueName(),

                        };
                        return countryItem.objectTrackingGridAPI.load(query);
                    };

                    for (var i = 0; i < drillDownDefinitions.length; i++)
                    {
                        DrillDownDefinitionsArray.push(drillDownDefinitions[i]);
                    }
                    DrillDownDefinitionsArray.push(drillDownDefinition);
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCountryAdded = function (countryObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(countryObject);
                        gridAPI.itemAdded(countryObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CountryAPIService.GetFilteredCountries(dataRetrievalInput)
                    .then(function (response) {   
                        if (response.Data != undefined)
                        {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();            
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCountry,
                haspermission: hasEditCountryPermission
            }];
        }

        function hasEditCountryPermission() {
            return VRCommon_CountryAPIService.HasEditCountryPermission();
        }

        function editCountry(countryObj) {
            var onCountryUpdated = function (countryObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(countryObj);
                gridAPI.itemUpdated(countryObj);
            };

            VRCommon_CountryService.editCountry(countryObj.Entity.CountryId, onCountryUpdated);
        }
              
    }

    return directiveDefinitionObject;

}]);
