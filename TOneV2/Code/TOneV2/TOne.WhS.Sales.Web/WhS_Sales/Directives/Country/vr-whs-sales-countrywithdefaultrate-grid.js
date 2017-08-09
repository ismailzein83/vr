
"use strict";

app.directive("vrWhsSalesCountrywithdefaultrateGrid", ["UtilsService", "VRNotificationService", "WhS_Sales_CountryWithDefaultRateAPIService", "WhS_Sales_CountryWithDefaultRateService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_Sales_CountryWithDefaultRateAPIService, WhS_Sales_CountryWithDefaultRateService, VRUIUtilsService) {

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
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Country/Templates/CountryWithDefaultRatesGridTemplate.html'

    };

    function CountryGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        var DrillDownDefinitionsArray = [];
        var countryPayload;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.countries = [];
            $scope.gridMenuActions;
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = WhS_Sales_CountryWithDefaultRateService.getDrillDownDefinition();
                registerZoneDrillDownToCountry();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(DrillDownDefinitionsArray, gridAPI, $scope.gridMenuActions);

                function registerZoneDrillDownToCountry() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Zones with default rate";
                    drillDownDefinition.directive = "vr-whs-sales-zonewithdefaultrate-grid";


                    drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {

                        countryItem.zoneGridAPI = directiveAPI;
                        var query = {
                            CountryId: countryItem.Entity.CountryId,
                            OwnerType:countryPayload.OwnerType ,
                            OwnerId: countryPayload.OwnerId,

                        };
                        return countryItem.zoneGridAPI.load(query);
                    };

                    for (var i = 0; i < drillDownDefinitions.length; i++) {
                        DrillDownDefinitionsArray.push(drillDownDefinitions[i]);
                    }
                    DrillDownDefinitionsArray.push(drillDownDefinition);
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        countryPayload=query;
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_CountryWithDefaultRateAPIService.GetFilteredCountries(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
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
        }

    }

    return directiveDefinitionObject;

}]);
