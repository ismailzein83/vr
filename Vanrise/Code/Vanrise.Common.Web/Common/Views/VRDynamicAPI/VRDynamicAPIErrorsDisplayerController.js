(function (appControllers) {

    "use strict";
    vrDynamicAPIErrorsDisplayerController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIErrorsDisplayerController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
       
        var errors;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                errors = parameters.errors;
            }
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onVRDynamicAPIErrorsGridReady = function (api) {
                api.load({errors:errors});
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Errors Displayer";
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

    }

    appControllers.controller('VR_Dynamic_API_Errors_DisplayerController', vrDynamicAPIErrorsDisplayerController);

})(appControllers);