appControllers.directive("vanriseToolsTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Tools_TableDataAPIService","VR_Tools_ColumnsAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService","VRCommon_VRBulkActionDraftService",
function (UtilsService, VRNotificationService, VR_Tools_TableDataAPIService,VR_Tools_ColumnsAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService,VRCommon_VRBulkActionDraftService) {
    "use strict"

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
        templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/GeneratedScript/Design/Templates/TableDataGridTemplate.html"
    };

    function TableDataGrid($scope, ctrl) {
    

        var gridApi;
        $scope.scopeModel = {};
        $scope.scopeModel.tableData = [];
        $scope.scopeModel.columnNames = [];
        var name;
        var pushedDataRows = [];
        var context;
        var bulkActionDraftInstance;
        var gridQuery;
        var identifierKey;
        var query;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }
                function getColumnNames(filter) {
                    return VR_Tools_ColumnsAPIService.GetColumnsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
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
                        context = payload.context;
                        //context.setActionsEnablity(true);
                         query = payload.query; 
                        gridQuery = query;
                        getColumnNames(query).then(function (response) {
                            bulkActionDraftInstance = VRCommon_VRBulkActionDraftService.createBulkActionDraft(getContext());
                             gridApi.retrieveData(query);
                        });
                    };

                    directiveApi.getData = function () {
                        if (gridQuery) {
                            gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                            gridQuery.BulkActionFinalState = bulkActionDraftInstance.finalizeBulkActionDraft().$$state.value;


                            return { gridQuery: gridQuery, columnNames: $scope.scopeModel.columnNames };
                        };
                    };

                    directiveApi.deselectAllAcounts=function(){

                        bulkActionDraftInstance.deselectAllItems();
                    };

                    directiveApi.selectAllAcounts=function(){

                        bulkActionDraftInstance.selectAllItems();
                    };

                    directiveApi.finalizeBulkActionDraft=function(){

                        bulkActionDraftInstance.finalizeBulkActionDraft();
                    };

                    return directiveApi;
                };
            }

            function getContext() {
                var currentContext=context
                if (currentContext==undefined)
                 currentContext = {};
                currentContext.triggerRetrieveData = function () {

                    gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                    gridApi.retrieveData(gridQuery)
                }
                currentContext.hasItems = function () {

                    return $scope.scopeModel.tableData.length > 0;
                };

               
                return currentContext;
            }

            function prepareRow(row) {

                var identifierKey = "";
                for (var j = 0; j < query.IdentifierColumns.length; j++) {
                    var identifierColumn = query.IdentifierColumns[j].ColumnName;
                    identifierKey += row.FieldValues[identifierColumn] + "_";
                }
                row.isSelected = bulkActionDraftInstance.isItemSelected(identifierKey);
                row.onSelectItem = function () {
                    bulkActionDraftInstance.onSelectItem({ ItemId: identifierKey }, row.isSelected);
                };
            }

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Tools_TableDataAPIService.GetFilteredTableData(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var row = response.Data[i];
                                prepareRow(row);
                            }
                        };
                        onResponseReady(response);
                        bulkActionDraftInstance.reEvaluateButtonsStatus();
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        };

 

    }

    return directiveDefinitionObject;

}]);
