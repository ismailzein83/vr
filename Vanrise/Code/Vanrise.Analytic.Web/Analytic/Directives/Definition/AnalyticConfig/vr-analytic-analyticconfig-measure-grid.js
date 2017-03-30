"use strict";

app.directive("vrAnalyticAnalyticconfigMeasureGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticMeasureGrid = new AnalyticMeasureGrid($scope, ctrl, $attrs);
            analyticMeasureGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticMeasureGridTemplate.html"

    };

    function AnalyticMeasureGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        var tableId;
        var itemType;
        function initializeController() {
            $scope.measures = [];

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
                    directiveAPI.onAnalyticMeasureAdded = function (measureObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(measureObj);
                        gridAPI.itemAdded(measureObj);
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
                clicked: editMeasure,
            }];
        }

        function editMeasure(dataItem) {
            var onEditMeasure = function (measureObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(measureObj);
                gridAPI.itemUpdated(measureObj);
            };

            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditMeasure, tableId, itemType);

        }


    }

    return directiveDefinitionObject;

}]);