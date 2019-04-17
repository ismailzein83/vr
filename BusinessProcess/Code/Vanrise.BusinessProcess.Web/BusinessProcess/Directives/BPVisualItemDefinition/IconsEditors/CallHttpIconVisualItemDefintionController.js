(function (appControllers) {

    "use strict";

    CallHttpIconVisualItemDefintionController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','VRNotificationService', 'VisualEventTypeEnum'];

    function CallHttpIconVisualItemDefintionController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VisualEventTypeEnum) {
        var events = [];

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

            $scope.scopeModel.onTrackingEventsDirectiveReady = function (api) {
                trakingEventsDirectiveAPI = api;
                trackingEventsPromiseReadyDeffered.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }


        function load() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadTrackingEventsDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
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
    appControllers.controller('BusinessProcess_BP_CallHttpIconVisualItemDefintionController', CallHttpIconVisualItemDefintionController);
})(appControllers);

