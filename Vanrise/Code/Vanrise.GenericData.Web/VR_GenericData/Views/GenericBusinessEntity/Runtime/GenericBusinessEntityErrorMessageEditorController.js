(function (appControllers) {

    'use strict';

    GenericBusinessEntityErrorMessageEditorController.$inject = ['$scope', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityErrorMessageEditorController($scope, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

  
        var errorEntity;
        var className = 'alert alert-danger';
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                errorEntity = parameters.errorEntity;
            }
        }

        function defineScope() {
            $scope.title = "Error Message";
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.className = "alert alert-danger";

        }

        function load() {
            if (errorEntity != undefined)
                $scope.scopeModel.error = errorEntity.message;
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

               

                return UtilsService.waitMultiplePromises([]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityErrorMessageEditorController', GenericBusinessEntityErrorMessageEditorController);

})(appControllers);