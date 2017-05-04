"use strict";

app.directive("vrCommonOverriddenconfigGrid", ["UtilsService", "VRNotificationService", "VRCommon_OverriddenConfigAPIService", "VRCommon_OverriddenConfigService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRCommon_OverriddenConfigAPIService, VRCommon_OverriddenConfigService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var overriddenConfigGrid = new OverriddenConfigGrid($scope, ctrl, $attrs);
            overriddenConfigGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/OverriddenConfig/Templates/OverriddenConfigGrid.html"

    };

    function OverriddenConfigGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.overridenConfigs = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;
                var drillDownDefinitions = VRCommon_OverriddenConfigService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onOverriddenConfigAdded = function (overriddenConfigObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(overriddenConfigObject);
                        gridAPI.itemAdded(overriddenConfigObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_OverriddenConfigAPIService.GetFilteredOverriddenConfigurations(dataRetrievalInput)
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
                clicked: editOverriddenConfig,
            }];
        }

        function editOverriddenConfig(overriddenConfigObject) {
            var onOverriddenConfigUpdated = function (overriddenConfigObject) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(overriddenConfigObject);
                gridAPI.itemUpdated(overriddenConfigObject);
            };

            VRCommon_OverriddenConfigService.editOverriddenConfig(overriddenConfigObject.OverriddenConfigurationId, onOverriddenConfigUpdated);
        }

    }

    return directiveDefinitionObject;

}]);