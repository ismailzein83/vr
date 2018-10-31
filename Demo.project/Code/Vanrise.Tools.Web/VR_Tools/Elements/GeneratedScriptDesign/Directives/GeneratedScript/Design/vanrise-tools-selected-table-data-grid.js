appControllers.directive("vanriseToolsSelectedTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Tools_TableDataAPIService","VR_Tools_ColumnsAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VR_Tools_TableDataAPIService,VR_Tools_ColumnsAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
    "use strict"

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var selectedTableDataGrid = new SelectedTableDataGrid($scope, ctrl, $attrs);
            selectedTableDataGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/GeneratedScript/Design/Templates/SelectedTableDataGridTemplate.html"
    };

    function SelectedTableDataGrid($scope, ctrl) {
    

        var gridApi;
        $scope.scopeModel = {};
        $scope.scopeModel.selectedTableData = [];
        $scope.scopeModel.columnNames = [];
        var tableRows=[];
        var name;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }
     

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        if (payload != undefined ) {

                            var columnNames = payload.ColumnNames;
                            $scope.scopeModel.columnNames = [];

                            for (var j = 0; j < columnNames.length; j++) {
                                $scope.scopeModel.columnNames.push(columnNames[j]);
                            }

                            var filter = payload.Query; 
                            if (filter.BulkActionFinalState.TargetItems.length != 0) {
                                return VR_Tools_TableDataAPIService.GetSelectedTableData(filter).then(function (response) {
                                    if (response) {
                                        tableRows = [];
                                        $scope.scopeModel.selectedTableData = [];
                                        for (var i = 0; i < response.length; i++) {
                                            tableRows.push(response[i]);
                                            $scope.scopeModel.selectedTableData.push(response[i])
                                        } 

                                    };
                                });
                            }
                            else {
                                $scope.scopeModel.selectedTableData = [];
                            }
                        }
                    };

                    directiveApi.getData = function () {
                        return { tableRows: tableRows }

                    };
                    return directiveApi;
                };
            }
            defineMenuActions();

        };
        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Delete",
                clicked: deleteTableDataRow,
            }];
        }

        function deleteTableDataRow(row) {

            var index = $scope.scopeModel.selectedTableData.indexOf(row)
            if (index > -1) {
                $scope.scopeModel.selectedTableData.splice(index, 1);
            }
        }
      
    }

    return directiveDefinitionObject;

}]);
