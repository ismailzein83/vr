(function (appControllers) {
    'use strict';

    QueueItemHeaderController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function QueueItemHeaderController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var filter = {};

        var executionFlowSelectorAPI;
        var executionFlowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var queueInstanceSelectorAPI;
        var queueInstanceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var queueStatusSelectorAPI;
        var queueStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {

            $scope.selectedExecutionFlow = [];
            $scope.selectedQueueInstance = [];
            $scope.selectedQueueStatus = [];

            $scope.ShowGrid = false;

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


            $scope.onExecutionFlowSelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoading = value };
                    var payload = {
                        executionFlowId: selectedItem.ExecutionFlowId
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, queueInstanceSelectorAPI, payload,setLoader);
                }

            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
      
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
                    selectedIds: null
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
        }
    }

    appControllers.controller('VR_Queueing_QueueItemHeaderController', QueueItemHeaderController);

})(appControllers);
