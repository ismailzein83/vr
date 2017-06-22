(function (appControllers) {

    "use strict";

    BusinessProcess_BP_InstanceRunningProcessController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceService', 'VRNotificationService', 'VRNavigationService'];

    function BusinessProcess_BP_InstanceRunningProcessController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceService, VRNotificationService, VRNavigationService) {
        var gridAPI;
        var context;
        var entityIds;
        var runningInstanceEditorSettings;
        loadParameters();
        defineScope();
        loadAllControls();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                context = parameters.context;
                entityIds = parameters.EntityIds;
                runningInstanceEditorSettings = parameters.runningInstanceEditorSettings;
            }
        }
        function defineScope() {
            $scope.message = runningInstanceEditorSettings.message;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var query = {
                    EntityIds: entityIds
                };
                gridAPI.loadGrid(query);
            };

            $scope.modalContext.onModalHide = function () {

                if (context != undefined) {
                    if (context.onClose != undefined) {
                        context.onClose();
                    }
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = "Running Processes";
        }
    }

        appControllers.controller('BusinessProcess_BP_InstanceRunningProcessesController', BusinessProcess_BP_InstanceRunningProcessController);
})(appControllers);