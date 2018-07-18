"use strict";

app.directive("vrQueueingQueueitemheaderGrid", ["UtilsService", "VRNotificationService", "VR_Queueing_QueueingAPIService", "VR_Integration_DataSourceService",
    function (UtilsService, VRNotificationService, VR_Queueing_QueueingAPIService, VR_Integration_DataSourceService) {

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
            templateUrl: "/Client/Modules/Queueing/Directives/QueueItemHeader/Templates/QueueItemHeaderGridTemplate.html"
        };

        function ImportedBatchGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.queueItemHeaders = [];

                $scope.gridReady = function (api) {
                    gridAPI = api;

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

                    return VR_Queueing_QueueingAPIService.GetQueueItemHeaders(dataRetrievalInput)
                        .then(function (response) {

                            angular.forEach(response.Data, function (item) {
                                item.Entity.ExecutionStatusDescription = VR_Integration_DataSourceService.getExecutionStatusDescription(item.Entity.Status);
                            });

                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                $scope.getStatusColor = function (dataItem, colDef) {
                    return VR_Integration_DataSourceService.getExecutionStatusColor(dataItem.Status);
                };
            }
        }

        return directiveDefinitionObject;
    }]);