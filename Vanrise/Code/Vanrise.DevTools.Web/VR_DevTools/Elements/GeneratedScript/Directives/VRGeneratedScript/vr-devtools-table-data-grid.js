appControllers.directive("vrDevtoolsTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Devtools_TableDataAPIService", "VR_Devtools_ColumnsAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VRCommon_VRBulkActionDraftService", "VR_Devtools_GeneratedScriptService","LabelColorsEnum",
    function (UtilsService, VRNotificationService, VR_Devtools_TableDataAPIService, VR_Devtools_ColumnsAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VRCommon_VRBulkActionDraftService, VR_Devtools_GeneratedScriptService, LabelColorsEnum) {
        "use strict";

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var tableDataGrid = new TableDataGrid($scope, ctrl, $attrs);
                tableDataGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScript/Templates/VRTableDataGridTemplate.html"
        };

        function TableDataGrid($scope, ctrl) {


            $scope.scopeModel = {};
            $scope.scopeModel.tableData = [];
            $scope.scopeModel.tableDataGridDS = [];
            $scope.scopeModel.columnNames = [];

            var gridApi;
            var context;
            var bulkActionDraftInstance;
            var gridQuery;
            var query;
            var selectedTableDataContext;
            var getNonIdentifierColumnNames;
            var compareTables;
            var generateSelectedTableDataGrid;
            this.initializeController = initializeController;

            function initializeController() {



                $scope.scopeModel.disableSelectAll = true;

                $scope.scopeModel.disableDeSelectAll = true;

                $scope.scopeModel.selectAll = function () {

                    bulkActionDraftInstance.selectAllItems();

                    for (var i = 0; i < $scope.scopeModel.tableData.length; i++) {
                        $scope.scopeModel.tableData[i].isSelected = true;
                    } 
                    gridQuery.allSelected = true;
                };

                $scope.scopeModel.deSelectAll = function () {

                    bulkActionDraftInstance.deselectAllItems();

                    for (var i = 0; i < $scope.scopeModel.tableData.length; i++) {
                        $scope.scopeModel.tableData[i].isSelected = false;
                    } 
                    gridQuery.allSelected = false;
                };

                $scope.scopeModel.getRowStyle = function (row) {
                    var rowStyle;
                    if (row) {
                        if (row.rowExists == undefined || row.rowExists == false)
                            rowStyle = { CssClass: 'alert-danger' };
                        return rowStyle;
                    }
                };

                $scope.getStatusColor = function (dataItem, column) {

                    if (dataItem.DescriptionEntity[column.name] != undefined && dataItem.DescriptionEntity[column.name].isDifferent)
                        return LabelColorsEnum.Processing.color;
                };

                $scope.scopeModel.generateSelectedTableDataGrid = function () {
                    getGridQuery(); 

                    var payload = {
                        Query: gridQuery,
                        tableData: $scope.scopeModel.tableData,
                        context: selectedTableDataContext,
                        moveItems: true
                    };
                    VR_Devtools_GeneratedScriptService.chooseSelectedTableDataColumns(payload, $scope.scopeModel.deSelectAll, getNonIdentifierColumnNames,generateSelectedTableDataGrid);

                };

                $scope.scopeModel.disablegenerateSelectedTableDataGrid = function () {
                    getGridQuery();
                    if (gridQuery == undefined || gridQuery.BulkActionFinalState == undefined || gridQuery.BulkActionFinalState.TargetItems == undefined || (gridQuery.BulkActionFinalState.TargetItems.length == 0 && gridQuery.allSelected == false))
                        return true;
                    return false;
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreRows();
                };

                $scope.scopeModel.totalRows = function () {
                    return $scope.scopeModel.tableData.length;
                };
                $scope.scopeModel.newRows = function () {
                    var newRowsCount = 0;
                    for (var i = 0; i < $scope.scopeModel.tableData.length; i++) {
                        if (!$scope.scopeModel.tableData[i].rowExists)
                            newRowsCount++;
                    }
                    return newRowsCount;
                };
               
                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

    
                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            var loadPromise = UtilsService.createPromiseDeferred();
                            context = payload.context;
                            selectedTableDataContext = context.selectedTableDataContext;
                            context.setSelectAllEnablity = function (enablity) {
                                $scope.scopeModel.disableSelectAll = !enablity;
                            };
                            context.setDeselectAllEnablity = function (enablity) {
                                $scope.scopeModel.disableDeSelectAll = !enablity;
                            };
                            getNonIdentifierColumnNames = context.nonIdentifierColumnNames;
                            compareTables = context.selectedTableDataContext.compareTables;

                            query = payload.query;
                            gridQuery = query;
                            gridQuery.allSelected = false;
                            
                            $scope.scopeModel.columnNames = payload.columnNames;
                            $scope.scopeModel.key = 'FieldValues["' + $scope.scopeModel.columnNames[0].Name + '"]';

                            generateSelectedTableDataGrid = payload.generateSelectedTableDataGrid;
                            bulkActionDraftInstance = VRCommon_VRBulkActionDraftService.createBulkActionDraft(getContext());

                            VR_Devtools_TableDataAPIService.GetFilteredTableData(query).then(function (response) {
                                if (response) {
                                    $scope.scopeModel.tableData = [];
                                    $scope.scopeModel.tableDataGridDS.length=0;

                                    for (var i = 0; i < response.length; i++) {
                                        var row = response[i];
                                        prepareRow(row);
                                        $scope.scopeModel.tableData.push(row);
                                    } 
                                } 
                                bulkActionDraftInstance.reEvaluateButtonsStatus();
                                loadMoreRows();
                                loadPromise.resolve();
                            });

                            return loadPromise.promise;
                        };

                        directiveApi.getData = function () {
                            if (gridQuery) {
                                if (bulkActionDraftInstance) {
                                    gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                                    gridQuery.BulkActionFinalState = bulkActionDraftInstance.finalizeBulkActionDraft().$$state.value;
                                  
                                }
                            } 
                            return {
                                gridQuery: gridQuery,
                                tableData: $scope.scopeModel.tableData,
                            };
                        };

                        directiveApi.deselectAllAcounts = function () {

                            bulkActionDraftInstance.deselectAllItems();
                        };

                        directiveApi.selectAllAcounts = function () {

                            bulkActionDraftInstance.selectAllItems();
                        };

                        directiveApi.finalizeBulkActionDraft = function () {

                            bulkActionDraftInstance.finalizeBulkActionDraft();
                        };

                        return directiveApi;
                    }
                };

                function getContext() {
                    var currentContext = context;
                    if (currentContext == undefined)
                        currentContext = {};

                    currentContext.triggerRetrieveData = function () {
                        gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                        gridApi.retrieveData(gridQuery);
                    };
                    currentContext.hasItems = function () {
                        return $scope.scopeModel.tableData.length > 0;
                    };
                    return currentContext;
                }

                function prepareRow(row) {

                    var identifierKey = "";
                    for (var j = 0; j < query.IdentifierColumns.length; j++) {
                        var identifierColumn = query.IdentifierColumns[j].ColumnName;
                        if (row.FieldValues[identifierColumn] != undefined && row.FieldValues[identifierColumn] != null && row.FieldValues[identifierColumn] != "")
                            identifierKey += row.FieldValues[identifierColumn] + "_";
                    }
                    row.identifierKey = identifierKey;
                    
                    row.isSelected = bulkActionDraftInstance.isItemSelected(identifierKey);
                   
                    row.onSelectItem = function () {
                        bulkActionDraftInstance.onSelectItem({ ItemId: row.identifierKey },row.isSelected);
                    };
                    row.DescriptionEntity = {};
                    //for (var i = 0; i < $scope.scopeModel.columnNames.length; i++) {
                    //    var columnName = $scope.scopeModel.columnNames[i];
                    //    row.DescriptionEntity[columnName] = { value: row[i].FieldValues[columnName]};
                    //}
                }
     
                function getGridQuery() {
                    if (gridQuery) {
                        if (bulkActionDraftInstance) {
                            gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                            gridQuery.BulkActionFinalState = bulkActionDraftInstance.finalizeBulkActionDraft().$$state.value;

                        }
                    }


                }


                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                };
            }
            function loadMoreRows() {

                var pageInfo = gridApi.getPageInfo();
                var itemsLength = pageInfo.toRow; 
                if (pageInfo.toRow > $scope.scopeModel.tableData.length) {
                    if (pageInfo.fromRow <= $scope.scopeModel.tableData.length) { itemsLength = $scope.scopeModel.tableData.length; }
                    else
                        return;
                }
                var items = [];
                for (var i = pageInfo.fromRow - 1; i < itemsLength; i++) {
                    items.push($scope.scopeModel.tableData[i]);
                } 
                gridApi.addItemsToSource(items);
                compareTables();
            }
        }

        return directiveDefinitionObject;

    }]);
