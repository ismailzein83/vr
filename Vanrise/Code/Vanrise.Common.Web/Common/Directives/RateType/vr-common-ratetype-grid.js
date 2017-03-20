"use strict";
app.directive("vrCommonRatetypeGrid", ["UtilsService", "VRNotificationService", "VRCommon_RateTypeAPIService", "VRCommon_RateTypeService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRCommon_RateTypeAPIService, VRCommon_RateTypeService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var rateTypeGrid = new RateTypeGrid($scope, ctrl, $attrs);
            rateTypeGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/RateType/Templates/RateTypeGridTemplate.html"

    };

    function RateTypeGrid($scope, ctrl, $attrs) {
        var gridDrillDownTabsObj;
        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.rateTypes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = VRCommon_RateTypeService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onRateTypeAdded = function (rateTypeObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(rateTypeObject);
                        gridAPI.itemAdded(rateTypeObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput)
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
                clicked: editRateType,
                haspermission: hasUpdateRateTypePermission
            }];
        }

        function hasUpdateRateTypePermission() {
            return VRCommon_RateTypeAPIService.HasUpdateRateTypePermission();
        }

        function editRateType(rateType) {
            var onRateTypeUpdated = function (updatedRateType) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedRateType);
                gridAPI.itemUpdated(updatedRateType);
            };
            VRCommon_RateTypeService.editRateType(rateType.Entity.RateTypeId, onRateTypeUpdated);
        }
    }

    return directiveDefinitionObject;

}]);