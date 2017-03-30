"use strict";

app.directive("vrAnalyticAnalyticconfigJoinGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticJoinGrid = new AnalyticJoinGrid($scope, ctrl, $attrs);
            analyticJoinGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticJoinGridTemplate.html"

    };

    function AnalyticJoinGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        var tableId;
        var itemType;
        function initializeController() {
            $scope.joins = [];

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
                    directiveAPI.onAnalyticJoinAdded = function (joinObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(joinObj);
                        gridAPI.itemAdded(joinObj);
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
                clicked: editJoin,
            }];
        }

        function editJoin(dataItem) {
            var onEditJoin = function (joinObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(joinObj);
                gridAPI.itemUpdated(joinObj);
            };

            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditJoin, tableId, itemType);

        }


    }

    return directiveDefinitionObject;

}]);