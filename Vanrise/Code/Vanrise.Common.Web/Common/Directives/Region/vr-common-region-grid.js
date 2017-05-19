"use strict";

app.directive("vrCommonRegionGrid", ["UtilsService", "VRNotificationService", "VRCommon_RegionAPIService", "VRCommon_RegionService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRCommon_RegionAPIService, VRCommon_RegionService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var regionGrid = RegionGrid($scope, ctrl, $attrs);
            regionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Region/Templates/RegionGridTemplate.html"

    };

    function RegionGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        return { initializeController: initializeController };
        function initializeController() {

            $scope.regions = [];
            $scope.onGridReady = function (api) {
             
                gridAPI = api;
                var drillDownDefinitions = VRCommon_RegionService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onRegionAdded = function (RegionObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(RegionObject);
                        gridAPI.itemAdded(RegionObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_RegionAPIService.GetFilteredRegions(dataRetrievalInput)
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
                clicked: editRegion,
                haspermission: hasEditRegionPermission
            }];
        }

        function hasEditRegionPermission() {
            return VRCommon_RegionAPIService.HasEditRegionPermission();
        }

        function editRegion(regionObj) {
            var onRegionUpdated = function (RegionObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(RegionObj);
                gridAPI.itemUpdated(RegionObj);
            };
            VRCommon_RegionService.editRegion(regionObj.Entity.RegionId, onRegionUpdated);
        }

    }

    return directiveDefinitionObject;

}]);