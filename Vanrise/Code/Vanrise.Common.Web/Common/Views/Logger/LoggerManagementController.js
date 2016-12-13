(function (appControllers) {

    "use strict";

    loggerManagementController.$inject = ['$scope', 'VRCommon_LogAttributeEnum', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService'];

    function loggerManagementController($scope, VRCommon_LogAttributeEnum, VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService) {
        var gridAPI;
        var filter = {};

        var logSearchDirectiveApi;
        var logSearchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {           
            $scope.onEntryTypeDirectiveReady = function (api) {
                logSearchDirectiveApi = api;
                logSearchReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadLogSearchDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadLogSearchDirective() {
            var logSearchLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            
            logSearchReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(logSearchDirectiveApi, undefined, logSearchLoadPromiseDeferred);
                });
            return logSearchLoadPromiseDeferred.promise;
        }

    }

    appControllers.controller('VRCommon_LoggerManagementController', loggerManagementController);
})(appControllers);