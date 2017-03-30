"use strict";

app.directive("vrAnalyticAnalyticconfigDimensionGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticDimensionGrid = new AnalyticDimensionGrid($scope, ctrl, $attrs);
            analyticDimensionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticDimensionGridTemplate.html"

    };

    function AnalyticDimensionGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        var tableId;
        var itemType;
        function initializeController() {
            $scope.dimensions = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = VR_Analytic_AnalyticItemConfigService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        if (query != undefined) {
                            tableId = query.TableId;
                            itemType = query.ItemType;
                        }
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onAnalyticDimensionAdded = function (dimensionObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(dimensionObj);
                        gridAPI.itemAdded(dimensionObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Analytic_AnalyticItemConfigAPIService.GetFilteredAnalyticItemConfigs(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editDimension,
            }];
        }

        function editDimension(dataItem) {
            var onEditDimension = function (dimensionObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(dimensionObj);
                gridAPI.itemUpdated(dimensionObj);
            };

            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditDimension, tableId, itemType);

        }


    }

    return directiveDefinitionObject;

}]);