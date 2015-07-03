(function(appControllers) {
    
    "use strict";

    appControllers.controller('Queueing_QueueItemManagementController', queueItemManagementController);

    queueItemManagementController.$inject = ['$scope', 'QueueingAPIService', 'UtilsService', 'QueueItemStatusEnum', 'LabelColorsEnum'];

    function queueItemManagementController($scope, QueueingAPIService, UtilsService, QueueItemStatusEnum, LabelColorsEnum) {

        var mainGridApi;
        
        function getCurrentDate(days) {
            var d = new Date();
            var curr_date = d.getDate() + days;
            var curr_month = d.getMonth();
            var curr_year = d.getFullYear();
            return new Date(curr_year, curr_month, curr_date);
        }

        function getData() {

            var pageInfo = mainGridApi.getPageInfo();
            return QueueingAPIService.GetHeaders(UtilsService.getPropValuesFromArray($scope.selectedQueueInstances, "QueueInstanceId"),
                pageInfo.fromRow,
                pageInfo.toRow,
                UtilsService.getPropValuesFromArray($scope.selectedQueueItemStatus, "Value"),
                $scope.fromDate,
                $scope.toDate).then(function (response) {
                    mainGridApi.addItemsToSource(response);
            });
        }

        function defineScope() {
            $scope.toDate = getCurrentDate(+1);
            $scope.fromDate = getCurrentDate(-1);
            $scope.queueItemStatus = [];
            $scope.selectedQueueItemStatus = [];
            $scope.queueItemTypes = [];
            $scope.selectedQueueItemTypes = [];
            $scope.queueInstances = [];
            $scope.selectedQueueInstances = [];
            $scope.onchangeQueueItemTypes = function () {
                QueueingAPIService.GetQueueInstances(UtilsService.getPropValuesFromArray($scope.selectedQueueItemTypes, "Id")).then(function (response) {
                    $scope.queueInstances = [];
                    $scope.selectedQueueInstances = [];
                    for (var i = 0, len = response.length; i < len; i++) {
                        $scope.queueInstances.push(response[i]);
                    }
                });
            };
            $scope.onSearchClicked = function () {
                mainGridApi.clearDataAndContinuePaging();
                return getData();
            };
        }

        function defineGrid() {
            $scope.datasource = [];
            $scope.loadMoreData = function () {
                return getData();
            };
            $scope.onGridReady = function (api) {
                mainGridApi = api;
            };
            $scope.getStatusColor = function (dataItem, colDef) {

                if (dataItem.Status === QueueItemStatusEnum.New.value) return LabelColorsEnum.Primary.Color;
                if (dataItem.Status === QueueItemStatusEnum.Processing.value) return LabelColorsEnum.Info.Color;
                if (dataItem.Status === QueueItemStatusEnum.Failed.value) return LabelColorsEnum.Error.Color;
                if (dataItem.Status === QueueItemStatusEnum.Processed.value) return LabelColorsEnum.Success.Color;

                return LabelColorsEnum.Info.Color;
            };

        }

        function loadFilters() {

            QueueingAPIService.GetQueueItemTypes().then(function (response) {

                for (var i = 0, len = response.length; i < len; i++) {
                    $scope.queueItemTypes.push(response[i]);
                }
            });

            QueueingAPIService.GetItemStatusList().then(function (response) {

                for (var i = 0, len = response.length; i < len; i++) {
                    $scope.queueItemStatus.push(response[i]);
                }
            });

            
        }

        defineScope();
        loadFilters();
        defineGrid();
    }

})(appControllers);