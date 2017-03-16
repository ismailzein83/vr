"use strict";

app.directive("vrCommonCityGrid", ["UtilsService", "VRNotificationService", "VRCommon_CityAPIService", "VRCommon_CityService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRCommon_CityAPIService, VRCommon_CityService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var cityGrid = new CityGrid($scope, ctrl, $attrs);
            cityGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/City/Templates/CityGridTemplate.html"

    };

    function CityGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.cities = [];
            $scope.onGridReady = function (api) {
             
                gridAPI = api;
                var drillDownDefinitions = VRCommon_CityService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCityAdded = function (cityObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(cityObject);
                        gridAPI.itemAdded(cityObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CityAPIService.GetFilteredCities(dataRetrievalInput)
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
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCity,
                haspermission: hasEditCityPermission
                }];
        }

        function hasEditCityPermission() {
            return VRCommon_CityAPIService.HasEditCityPermission();
        }

        function editCity(cityObj) {
            var onCityUpdated = function (cityObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(cityObj);
                gridAPI.itemUpdated(cityObj);
            };

            VRCommon_CityService.editCity(cityObj.Entity.CityId, onCityUpdated);
        }

    }

    return directiveDefinitionObject;

}]);