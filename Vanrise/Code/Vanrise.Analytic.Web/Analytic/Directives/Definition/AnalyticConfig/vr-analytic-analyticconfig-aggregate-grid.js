"use strict";

app.directive("vrAnalyticAnalyticconfigAggregateGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticDimensionGrid = new AnalyticAggregateGrid($scope, ctrl, $attrs);
            analyticDimensionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticAggregateGridTemplate.html"

    };

    function AnalyticAggregateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        var tableId;
        var itemType;
        function initializeController() {
            $scope.aggregates = [];

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
                    directiveAPI.onAnalyticAggregateAdded = function (aggregateObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(aggregateObj);
                        gridAPI.itemAdded(aggregateObj);
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
                clicked: editAggregate,
            }];
        }

        function editAggregate(dataItem) {
            var onEditAggregate = function (aggregateObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(aggregateObj);
                gridAPI.itemUpdated(aggregateObj);
            };

            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditAggregate, tableId, itemType);

        }


    }

    return directiveDefinitionObject;

}]);