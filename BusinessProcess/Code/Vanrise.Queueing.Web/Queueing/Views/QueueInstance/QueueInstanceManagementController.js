(function (appControllers) {
    'use strict';

    QueueInstanceController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function QueueInstanceController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var filter = {};

        var executionFlowSelectorAPI;
        var executionFlowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var stageNameSelectorAPI;
        var stageNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var itemTypeSelectorAPI;
        var itemTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {

            $scope.selectedExecutionFlow = [];
            $scope.selectedStageName=[];
            $scope.selectedQueueItemType = [];

            $scope.onExecutionFlowSelectorReady = function (api) {
                executionFlowSelectorAPI = api;
                executionFlowSelectorReadyDeferred.resolve();
            };


            $scope.onStageNameSelectorReady = function (api) {
                stageNameSelectorAPI = api;
                stageNameSelectorReadyDeferred.resolve();
            };


            $scope.onQueueItemTypeSelectorReady = function (api) {
                itemTypeSelectorAPI = api;
                itemTypeSelectorReadyDeferred.resolve();
            };


            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };


        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadExecutionFlow, loadStageName, loadItemType])
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

        function loadStageName() {
            var stageNameSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            stageNameSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(stageNameSelectorAPI, payload, stageNameSelectorLoadDeferred);
            });
            return stageNameSelectorLoadDeferred.promise;
        }


        function loadItemType() {
            var itemTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            itemTypeSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(itemTypeSelectorAPI, payload, itemTypeSelectorLoadDeferred);
            });
            return itemTypeSelectorLoadDeferred.promise;
        }



        function getFilterObject() {
            filter = {
                Name: $scope.name,
                ExecutionFlowId: executionFlowSelectorAPI.getSelectedIds(),
                StageName: stageNameSelectorAPI.getSelectedIds(),
                Title: $scope.title,
                ItemTypeId: itemTypeSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_Queueing_QueueInstanceController', QueueInstanceController);

})(appControllers);
