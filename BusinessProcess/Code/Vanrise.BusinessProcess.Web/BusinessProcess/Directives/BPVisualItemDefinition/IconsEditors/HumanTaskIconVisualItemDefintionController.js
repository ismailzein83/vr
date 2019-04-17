(function (appControllers) {

    "use strict";

    HumanTaskIconVisualItemDefintionController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VisualEventTypeEnum', 'BusinessProcess_BPTaskAPIService', 'BPTaskStatusEnum', 'BusinessProcess_BPTaskService'];

    function HumanTaskIconVisualItemDefintionController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VisualEventTypeEnum, BusinessProcess_BPTaskAPIService, BPTaskStatusEnum, BusinessProcess_BPTaskService) {

        var events = [];
        var taskId;

        var trakingEventsDirectiveAPI;
        var trackingEventsPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            events = parameters.events;
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.bpHumanTaskInstanceTracking = [];

            $scope.scopeModel.onTrackingEventsDirectiveReady = function (api) {
                trakingEventsDirectiveAPI = api;
                trackingEventsPromiseReadyDeffered.resolve();
            };
            $scope.scopeModel.onChangingTask = function () {
                BusinessProcess_BPTaskService.openTask(taskId);
                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }


        function load() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadTrackingEventsDirective, loadTaskEntity])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }


        function loadTaskEntity() {
            
            for (var i = 0; i < events.length; i++) {
                var event = events[i];
                if (event.EventPayload != undefined) {
                    taskId = event.EventPayload.TaskId;
                    break;
                }
            }
            if (taskId != undefined) {
                BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                    if (response != undefined) {
                        $scope.scopeModel.taskEntity = response;
                        $scope.scopeModel.taskEntity.StatusDescription = UtilsService.getEnumDescription(BPTaskStatusEnum, response.Status);
                    }
                });
            }
        }


        function loadTrackingEventsDirective() {
            var loadTrackingEventsPromiseDeffered = UtilsService.createPromiseDeferred();
            trackingEventsPromiseReadyDeffered.promise.then(function () {
                var payload = {
                    events: events != undefined && events.length > 0 ? events : undefined
                };
                VRUIUtilsService.callDirectiveLoad(trakingEventsDirectiveAPI, payload, loadTrackingEventsPromiseDeffered);
            });
            return loadTrackingEventsPromiseDeffered.promise;
        }

    }
    appControllers.controller('BusinessProcess_BP_HumanTaskIconVisualItemDefintionController', HumanTaskIconVisualItemDefintionController);
})(appControllers);

