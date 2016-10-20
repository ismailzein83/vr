"use strict";

app.directive("vrIntegrationDatasourceGrid", ["UtilsService", "VRNotificationService", "VR_Integration_DataSourceService",'VR_Integration_DataSourceAPIService',
function (UtilsService, VRNotificationService, VR_Integration_DataSourceService, VR_Integration_DataSourceAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataSourceGrid = new DataSourceGrid($scope, ctrl, $attrs);
            dataSourceGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Integration/Directives/DataSource/Templates/DataSourceGridTemplate.html"

    };

    function DataSourceGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        function initializeController() {

            $scope.getStatusColor = function (dataItem, colDef) {
                return VR_Integration_DataSourceService.getExecutionStatusColor(dataItem.Status);
            }
            $scope.dataSources = [];
            $scope.gridReady = function (api) {

                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onDataSourceAdded = function (dataSource) {

                        gridAPI.itemAdded(dataSource);

                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return VR_Integration_DataSourceAPIService.GetFilteredDataSources(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            }
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [
                {
                    name: "Edit",
                    clicked: editDataSource,
                    haspermission:hasEditDataSourcePermission
                },
                {
                    name: "Delete",
                    clicked: deleteDataSource,
                    haspermission: hasDeleteDataSourcePermission
                }
            ];
        }
        function editDataSource(dataSourceObj) {

            var onDataSourceUpdated = function (dataSource) {
                gridAPI.itemUpdated(dataSource);
            }

            VR_Integration_DataSourceService.editDataSource(dataSourceObj.Entity.DataSourceId, onDataSourceUpdated);
        }

        function hasEditDataSourcePermission() {
            return VR_Integration_DataSourceAPIService.HasUpdateDataSource();
        }

        function deleteDataSource(dataSourceObj) {
            var onDataSourceDeleted = function (dataSource) {
                gridAPI.itemDeleted(dataSource);
            }

            VR_Integration_DataSourceService.deleteDataSource($scope, dataSourceObj.Entity, onDataSourceDeleted);
        }

        function hasDeleteDataSourcePermission() {
            return VR_Integration_DataSourceAPIService.HasDeleteDataSource();
        }
    }

    return directiveDefinitionObject;

}]);
