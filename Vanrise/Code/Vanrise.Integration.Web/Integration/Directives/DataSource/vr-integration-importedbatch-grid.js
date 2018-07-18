"use strict";

app.directive("vrIntegrationImportedbatchGrid", ["UtilsService", "VRNotificationService", "VR_Integration_DataSourceImportedBatchAPIService", "VR_Integration_DataSourceService", 'VRUIUtilsService',
function (UtilsService, VRNotificationService, VR_Integration_DataSourceImportedBatchAPIService, VR_Integration_DataSourceService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var importedBatchGrid = new ImportedBatchGrid($scope, ctrl, $attrs);
            importedBatchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Integration/Directives/DataSource/Templates/ImportedBatchGridTemplate.html"
    };

    function ImportedBatchGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var gridDrillDownTabsObj;

        function initializeController() {
            $scope.importedBatches = [];

            $scope.gridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];

                var drillDownDefinition = {};
                drillDownDefinition.title = "Queue Items";
                drillDownDefinition.directive = "vr-queueing-queueitemheader-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, dataItem) {
                    dataItem.queueItemHeaderAPI = directiveAPI;
                    var query = dataItem.QueueItemIds.split(',').map(Number);
                        return dataItem.queueItemHeaderAPI.loadGrid(query);
                };
                drillDownDefinitions.push(drillDownDefinition);

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return VR_Integration_DataSourceImportedBatchAPIService.GetFilteredDataSourceImportedBatches(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var itm = response.Data[i];
                                itm.MappingResultDescription = VR_Integration_DataSourceService.getMappingResultDescription(itm.MappingResult);
                                itm.ExecutionStatusDescription = VR_Integration_DataSourceService.getExecutionStatusDescription(itm.ExecutionStatus);
                                gridDrillDownTabsObj.setDrillDownExtensionObject(itm);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            $scope.getStatusColor = function (dataItem, colDef) {
                return VR_Integration_DataSourceService.getExecutionStatusColor(dataItem.ExecutionStatus);
            };
        }
    }

    return directiveDefinitionObject;

}]);
