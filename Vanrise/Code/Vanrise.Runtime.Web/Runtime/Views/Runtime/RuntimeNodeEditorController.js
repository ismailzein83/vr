(function (appControllers) {
    'use strict';

    RuntimeNodeEditorController.$inject = ['$scope', 'VRRuntime_RuntimeNodeService', 'VRRuntime_RuntimeNodeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRRuntime_RuntimeNodeConfigurationAPIService'];

    function RuntimeNodeEditorController($scope, VRRuntime_RuntimeNodeService, VRRuntime_RuntimeNodeAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        defineScope();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.saveRuntimeNodeConfiguration = function () {
                    return insertRuntimeNodeConfiguration();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.isLoading = true;
                loadAllControls().catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
           $scope.title = UtilsService.buildTitleForAddEditor("Runtime Node");
        }

        function buildRuntimeNodeConfigurationObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                //selectordata
            };
            return obj;
        }

        function insertRuntimeNodeConfiguration() {
            $scope.isLoading = true;

            var runtimeNodeObject = buildRuntimeNodeObjFromScope();
            return VRRuntime_RuntimeNodeAPIService.AddRuntimeNode(runtimeNodeObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Runtime Node", response, "Name")) {
                    if ($scope.onRuntimeNodeAdded != undefined)
                        $scope.onRuntimeNodeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

    }

    appControllers.controller('VRRuntime_RuntimeNodeEditorController', RuntimeNodeEditorController);

})(appControllers);
