"use strict";

app.directive("vrAnalyticAnalytictableGrid", ['VRCommon_ObjectTrackingService', 'VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticTableService', 'UtilsService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VRUIUtilsService', 'VR_Analytic_AnalyticItemConfigService', function (VRCommon_ObjectTrackingService, VRNotificationService, VRModalService, VR_Analytic_AnalyticTableService, UtilsService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_AnalyticTypeEnum, VRUIUtilsService, VR_Analytic_AnalyticItemConfigService) {

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
            $scope.gridMenuActions = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onAnalyticTableAdded = function (tableObj) {
                        gridDrillDownTabs.setDrillDownExtensionObject(tableObj);
                        gridAPI.itemAdded(tableObj);
                    };
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
                haspermission: hasEditAnalyticTablePermission
            
            

            }];
        }
        function hasEditAnalyticTablePermission() {
            return VR_Analytic_AnalyticTableAPIService.HasEditAnalyticTablePermission();
        }
        function editTable(dataItem) {
            var onEditTable = function (tableObj) {
                gridDrillDownTabs.setDrillDownExtensionObject(tableObj);
                gridAPI.itemUpdated(tableObj);
            };
            VR_Analytic_AnalyticTableService.editAnalyticTable(dataItem.Entity.AnalyticTableId, onEditTable);
        }

        function getGridDrillDownDefinitions() {
            var drillDownDefinitions = [];
            drillDownDefinitions.push(getDimensionDrillDownDefinition());
            drillDownDefinitions.push(getMeasureDrillDownDefinition());
            drillDownDefinitions.push(getJoinDrillDownDefinition());
            drillDownDefinitions.push(getAggregateDrillDownDefinition());
            drillDownDefinitions.push(getDrillDownToAnalyticTable());
            drillDownDefinitions.push(getExternalSourceDrillDownDefinition()); 
            return drillDownDefinitions;
        }
        function getDimensionDrillDownDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = "Dimensions";
            drillDownDefinition.directive = "vr-analytic-analyticconfig-dimension-grid";
            drillDownDefinition.loadDirective = function (dimensionGridAPI, tableItem) {
                tableItem.dimensionGridAPI = dimensionGridAPI;
                var query = {
                    TableId: tableItem.Entity.AnalyticTableId,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value
                };
                return dimensionGridAPI.loadGrid(query);
            };

            drillDownDefinition.parentMenuActions = [{
                name: "Add Dimension",
                clicked: function (tableItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(tableItem);

                    var onDimensionAdded = function (dimensionObj) {
                        if (tableItem.dimensionGridAPI != undefined) {
                            tableItem.dimensionGridAPI.onAnalyticDimensionAdded(dimensionObj);
                        }
                    };
                    VR_Analytic_AnalyticItemConfigService.addItemConfig(onDimensionAdded, tableItem.Entity.AnalyticTableId, VR_Analytic_AnalyticTypeEnum.Dimension.value);
                },
            }];
            return drillDownDefinition;
        }
        function getMeasureDrillDownDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = "Measures";
            drillDownDefinition.directive = "vr-analytic-analyticconfig-measure-grid";
            drillDownDefinition.loadDirective = function (measureGridAPI, tableItem) {
                tableItem.measureGridAPI = measureGridAPI;

                var query = {
                    TableId: tableItem.Entity.AnalyticTableId,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value
                };
                return measureGridAPI.loadGrid(query);
            };

            drillDownDefinition.parentMenuActions = [{
                name: "Add Measure",
                clicked: function (tableItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(tableItem);

                    var onMeasureAdded = function (measureObj) {
                        if (tableItem.measureGridAPI != undefined) {
                            tableItem.measureGridAPI.onAnalyticMeasureAdded(measureObj);
                        }
                    };
                    VR_Analytic_AnalyticItemConfigService.addItemConfig(onMeasureAdded, tableItem.Entity.AnalyticTableId, VR_Analytic_AnalyticTypeEnum.Measure.value);
                },
            }];
            return drillDownDefinition;
        }
        function getDrillDownToAnalyticTable() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, analyticTableItem) {

                analyticTableItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: analyticTableItem.Entity.AnalyticTableId,
                    EntityUniqueName: VR_Analytic_AnalyticTableService.getEntityUniqueName(),

                };

                return analyticTableItem.objectTrackingGridAPI.load(query);
            };

            return drillDownDefinition;

        }
        function getJoinDrillDownDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = "Joins";
            drillDownDefinition.directive = "vr-analytic-analyticconfig-join-grid";
            drillDownDefinition.loadDirective = function (joinGridAPI, tableItem) {
                tableItem.joinGridAPI = joinGridAPI;

                var query = {
                    TableId: tableItem.Entity.AnalyticTableId,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Join.value
                };
                return joinGridAPI.loadGrid(query);
            };

            drillDownDefinition.parentMenuActions = [{
                name: "Add Join",
                clicked: function (tableItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(tableItem);

                    var onJoinAdded = function (joinObj) {
                        if (tableItem.joinGridAPI != undefined) {
                            tableItem.joinGridAPI.onAnalyticJoinAdded(joinObj);
                        }
                    };
                    VR_Analytic_AnalyticItemConfigService.addItemConfig(onJoinAdded, tableItem.Entity.AnalyticTableId, VR_Analytic_AnalyticTypeEnum.Join.value);
                },
            }];
            return drillDownDefinition;
        }
        function getAggregateDrillDownDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = "Aggregates";
            drillDownDefinition.directive = "vr-analytic-analyticconfig-aggregate-grid";
            drillDownDefinition.loadDirective = function (aggregateGridAPI, tableItem) {
                tableItem.aggregateGridAPI = aggregateGridAPI;

                var query = {
                    TableId: tableItem.Entity.AnalyticTableId,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Aggregate.value
                };
                return aggregateGridAPI.loadGrid(query);
            };

            drillDownDefinition.parentMenuActions = [{
                name: "Add Aggregate",
                clicked: function (tableItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(tableItem);

                    var onAggregateAdded = function (aggregateObj) {
                        if (tableItem.aggregateGridAPI != undefined) {
                            tableItem.aggregateGridAPI.onAnalyticAggregateAdded(aggregateObj);
                        }
                    };
                    VR_Analytic_AnalyticItemConfigService.addItemConfig(onAggregateAdded, tableItem.Entity.AnalyticTableId, VR_Analytic_AnalyticTypeEnum.Aggregate.value);
                },
            }];
            return drillDownDefinition;
        }
        function getExternalSourceDrillDownDefinition() { 
            var drillDownDefinition = {};
            drillDownDefinition.title = "External Sources";
            drillDownDefinition.directive = "vr-analytic-analyticconfig-measureexternalsource-grid";
            drillDownDefinition.loadDirective = function (externalSourceGridAPI, tableItem) {
                tableItem.externalSourceGridAPI = externalSourceGridAPI;

                var query = {
                    TableId: tableItem.Entity.AnalyticTableId,
                    ItemType: VR_Analytic_AnalyticTypeEnum.MeasureExternalSource.value
                };
                return externalSourceGridAPI.loadGrid(query);
            };
            drillDownDefinition.parentMenuActions = [{
                name: "Add External Source",
                clicked: function (tableItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(tableItem);
                    var onExternalSourceAdded = function (externalSourceObj) {
                        if (tableItem.externalSourceGridAPI != undefined) {
                            tableItem.externalSourceGridAPI.onAnalyticExternalSourceAdded(externalSourceObj);
                        }
                    };
                    VR_Analytic_AnalyticItemConfigService.addItemConfig(onExternalSourceAdded, tableItem.Entity.AnalyticTableId, VR_Analytic_AnalyticTypeEnum.MeasureExternalSource.value);
                }
            }];
            return drillDownDefinition;
        }
    }


    return directiveDefinitionObject;

}]);