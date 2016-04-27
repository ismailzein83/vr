"use strict";

app.directive("vrAnalyticAnalytictableGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticTableService', 'UtilsService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic__AnalyticTypeEnum','VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticTableService, UtilsService, VR_Analytic_AnalyticTableAPIService, VR_Analytic__AnalyticTypeEnum, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticTableGrid = new AnalyticTableGrid($scope, ctrl, $attrs);
            analyticTableGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticTable/Templates/AnalyticTableGridTemplate.html"

    };

    function AnalyticTableGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabs;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.tables = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, null);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onAnalyticTableAdded = function (tableObj) {
                        gridAPI.itemAdded(tableObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Analytic_AnalyticTableAPIService.GetFilteredAnalyticTables(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var tableItem = response.Data[i];
                                gridDrillDownTabs.setDrillDownExtensionObject(tableItem);
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
                clicked: editTable,
                //haspermission: hasUpdateViewPermission // System Entities:Assign Permissions
            }];
        }
        //function hasUpdateViewPermission() {
        //    return VR_Sec_ViewAPIService.HasUpdateViewPermission();
        //}
        function editTable(dataItem) {
            var onEditTable = function (tableObj) {
                gridAPI.itemUpdated(tableObj);
            }


            VR_Analytic_AnalyticTableService.editAnalyticTable(dataItem.Entity.AnalyticTableId, onEditTable);

        }

        function getGridDrillDownDefinitions() {
            return [{
                title: "Dimensions",
                directive: "vr-analytic-analyticconfig-dimension-grid",
                loadDirective: function (dimensionGridAPI, tableItem) {
                    var query = {
                        TableId: tableItem.Entity.AnalyticTableId,
                    };
                    return dimensionGridAPI.loadGrid(query);
                }
            }, {
                title: "Measures",
                directive: "vr-analytic-analyticconfig-measure-grid",
                loadDirective: function (measureGridAPI, tableItem) {
                    var query = {
                        TableId: tableItem.Entity.AnalyticTableId,
                    };
                    return measureGridAPI.loadGrid(query);
                }
            }, {
                title: "Joins",
                directive: "vr-analytic-analyticconfig-join-grid",
                loadDirective: function (joinGridAPI, tableItem) {
                    var query = {
                        TableId: tableItem.Entity.AnalyticTableId,
                    };
                    return joinGridAPI.loadGrid(query);
                }
            }];
        }
    }

    return directiveDefinitionObject;

}]);