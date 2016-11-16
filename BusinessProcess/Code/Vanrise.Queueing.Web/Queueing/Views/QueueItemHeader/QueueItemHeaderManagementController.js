(function (appControllers) {
    'use strict';

    QueueItemHeaderController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function QueueItemHeaderController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        var gridAPI;
        var filter = {};
        var isModalMode = false;
        var receivedQueueId = [];

        var executionFlowSelectorAPI;
        var executionFlowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var queueInstanceSelectorAPI;
        var queueInstanceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var queueStatusSelectorAPI;
        var queueStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var executionFlowIds = new Array();

        loadParameters();
        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                receivedQueueId.push(parameters.queueID);
                isModalMode = true;
            }
        }

        function defineScope() {

            $scope.selectedExecutionFlow = [];
            $scope.selectedQueueInstance = [];
            $scope.selectedQueueStatus = [];

            $scope.createdTimeFrom = new Date(new Date().setHours(0, 0, 0, 0));

            $scope.ShowGrid = false;
            $scope.isDisabledQueueInstanceSelect = false;

            $scope.onExecutionFlowSelectorReady = function (api) {
                executionFlowSelectorAPI = api;
                executionFlowSelectorReadyDeferred.resolve();
            };


            $scope.onQueueInstanceSelectorReady = function (api) {
                queueInstanceSelectorAPI = api;
                queueInstanceSelectorReadyDeferred.resolve();
            };


            $scope.onQueueStatusSelectorReady = function (api) {
                queueStatusSelectorAPI = api;
                queueStatusSelectorReadyDeferred.resolve();
            };




            $scope.onSelectedExecutionFlowChanged = function (selectedItem) {
                if (selectedItem != undefined) {

                    var setLoader = function (value) { $scope.isLoading = value };
                    if (executionFlowSelectorAPI.getSelectedIds() != undefined && executionFlowSelectorAPI.getSelectedIds().length > 1) {
                        $scope.isDisabledQueueInstanceSelect = true;
                        return;
                    }
                    else if (selectedItem != undefined && selectedItem.length > 0) {
                        $scope.isDisabledQueueInstanceSelect = false;
                        var payload = {
                            ExecutionFlowId: selectedItem[0].ExecutionFlowId,
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, queueInstanceSelectorAPI, payload, setLoader);
                    }
                    else {
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, queueInstanceSelectorAPI, {}, setLoader);
                    }



                }

            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (isModalMode && !$scope.isLoading) {
                    filter = getFilterObject();
                    gridAPI.loadGrid(filter);
                    $scope.ShowGrid = true;
                };
            };

            $scope.search = function () {
                getFilterObject();
                gridAPI.loadGrid(filter);
                $scope.ShowGrid = true;
            };


        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadExecutionFlow, loadQueueInstance, loadQueueStatus])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }


        function loadExecutionFlow() {
            var executionFlowSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            executionFlowSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(executionFlowSelectorAPI, payload, executionFlowSelectorLoadDeferred);
            });
            return executionFlowSelectorLoadDeferred.promise;
        }

        function loadQueueInstance() {
            var queueInstanceSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            queueInstanceSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: receivedQueueId
                };

                VRUIUtilsService.callDirectiveLoad(queueInstanceSelectorAPI, payload, queueInstanceSelectorLoadDeferred);
            });
            return queueInstanceSelectorLoadDeferred.promise;
        }


        function loadQueueStatus() {
            var queueStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            queueStatusSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(queueStatusSelectorAPI, payload, queueStatusSelectorLoadDeferred);
            });
            return queueStatusSelectorLoadDeferred.promise;
        }



        function getFilterObject() {
            filter = {
                ExecutionFlowIds: executionFlowSelectorAPI.getSelectedIds(),
                QueueIds: queueInstanceSelectorAPI.getSelectedIds(),
                QueueStatusIds: queueStatusSelectorAPI.getSelectedIds(),
                createdTimeFrom: $scope.createdTimeFrom,
                createdTimeTo: $scope.createdTimeTo
            };
            return filter;
        }
    }

    appControllers.controller('VR_Queueing_QueueItemHeaderController', QueueItemHeaderController);

})(appControllers);
