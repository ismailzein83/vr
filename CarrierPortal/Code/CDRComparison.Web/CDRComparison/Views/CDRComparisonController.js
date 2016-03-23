(function (appControllers) {

    'use strict';

    CDRComparisonController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function CDRComparisonController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {

        var systemDirectiveAPI;
        var systemDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.onCDRSourceSelectiveReady = function (api) {
                systemDirectiveAPI = api;
                systemDirectiveReadyDeferred.resolve();
            };

            $scope.start = function () {

            };
        }

        function load() {
            $scope.isLoading = true;

            return UtilsService.waitMultipleAsyncOperations([loadSystemDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadSystemDirective() {
            var systemDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            systemDirectiveReadyDeferred.promise.then(function () {
                var systemDirectivePayload;
                VRUIUtilsService.callDirectiveLoad(systemDirectiveAPI, systemDirectivePayload, systemDirectiveLoadDeferred);
            });

            return systemDirectiveLoadDeferred.promise;
        }
    }

    appControllers.controller('CDRComparison_CDRComparisonController', CDRComparisonController);

})(appControllers);