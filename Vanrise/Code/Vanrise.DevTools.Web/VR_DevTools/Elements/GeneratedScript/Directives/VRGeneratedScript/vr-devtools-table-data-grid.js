﻿appControllers.directive("vrDevtoolsTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Devtools_TableDataAPIService", "VR_Devtools_ColumnsAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VRCommon_VRBulkActionDraftService",
    function (UtilsService, VRNotificationService, VR_Devtools_TableDataAPIService, VR_Devtools_ColumnsAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VRCommon_VRBulkActionDraftService) {
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


            var gridApi;
            $scope.scopeModel = {};
            $scope.scopeModel.tableData = [];
            $scope.scopeModel.columnNames = [];
            var name;
            var context;
            var bulkActionDraftInstance;
            var gridQuery;
            var query;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.selectAll = function () {

                    bulkActionDraftInstance.selectAllItems();
                    gridQuery.allSelected = true;
                };

                $scope.scopeModel.deSelectAll = function () {

                    bulkActionDraftInstance.deselectAllItems();
                    gridQuery.allSelected = false;

                };

                $scope.scopeModel.disableSelectAll = true;

                $scope.scopeModel.disableDeSelectAll = true;

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }
                    function getColumnNames(filter) {
                        return VR_Devtools_ColumnsAPIService.GetColumnsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                            if (response != null) {
                                $scope.scopeModel.columnNames = [];
                                for (var i = 0; i < response.length; i++) {
                                    if (i == 0) {
                                        name = response[i].Name;
                                        $scope.scopeModel.key = 'FieldValues["' + name + '"]';
                                    }
                                    $scope.scopeModel.columnNames.push(response[i]);
                                }
                            }
                        });
                    }

                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            var loadPromise = UtilsService.createPromiseDeferred();
                            context = payload.context;
                            context.setSelectAllEnablity = function (enablity) {
                                $scope.scopeModel.disableSelectAll = !enablity;
                            };
                            context.setDeselectAllEnablity = function (enablity) {
                                $scope.scopeModel.disableDeSelectAll = !enablity;
                            };
                            query = payload.query;
                            gridQuery = query;
                            gridQuery.allSelected = false;

                            getColumnNames(query).then(function (response) {
                                bulkActionDraftInstance = VRCommon_VRBulkActionDraftService.createBulkActionDraft(getContext());
                                gridApi.retrieveData(query);
                                loadPromise.resolve();
                            });
                            return loadPromise.promise;
                        };

                        directiveApi.getData = function () {
                            if (gridQuery) {
                                if (bulkActionDraftInstance) {
                                    gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                                    gridQuery.BulkActionFinalState = bulkActionDraftInstance.finalizeBulkActionDraft().$$state.value;
                                    return { gridQuery: gridQuery, columnNames: $scope.scopeModel.columnNames };
                                }
                            }
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
                    row.isSelected = bulkActionDraftInstance.isItemSelected(identifierKey);
                    row.onSelectItem = function () {
                        bulkActionDraftInstance.onSelectItem({ ItemId: identifierKey }, row.isSelected);
                    };
                }

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Devtools_TableDataAPIService.GetFilteredTableData(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var row = response.Data[i];
                                    prepareRow(row);
                                }
                            }
                            onResponseReady(response);
                            bulkActionDraftInstance.reEvaluateButtonsStatus();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;

    }]);