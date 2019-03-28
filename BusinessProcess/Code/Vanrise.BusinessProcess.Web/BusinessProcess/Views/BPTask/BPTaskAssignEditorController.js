(function (appControllers) {

    "use strict";

    BusinessProcess_BP_TaskMonitorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','VRNavigationService'];

    function BusinessProcess_BP_TaskMonitorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {


        var userIds;

        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        loadAllControls();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                userIds = parameters.userIds;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.assign = function () {
                if ($scope.onUserAssigned != undefined && typeof ($scope.onUserAssigned) == 'function') {
                    $scope.onUserAssigned(userSelectorAPI.getSelectedIds());
                }
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }


        function loadAllControls() {
            $scope.isLoading = true;

            function setTitle() {
                $scope.title = 'Assign Task';
            }

            function loadUserSelector() {
                var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                userSelectorReadyDeferred.promise.then(function () {
                    var payload = {};
                    payload.filter = {
                        OnlyUserIds: userIds
                    };
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userSelectorLoadDeferred);
                });
                return userSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadUserSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }

    }

    appControllers.controller('BusinessProcess_BP_BPTaskAssignEditorController', BusinessProcess_BP_TaskMonitorController);
})(appControllers);