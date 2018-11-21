(function (appControllers) {

    'use strict';

    customCodeCompilationErrorsEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VR_Invoice_InvoiceFieldEnum', 'VRCommon_GridWidthFactorEnum', 'VRUIUtilsService'];

    function customCodeCompilationErrorsEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VR_Invoice_InvoiceFieldEnum, VRCommon_GridWidthFactorEnum, VRUIUtilsService) {

        var context;
        var errorMessages;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                errorMessages = parameters.errorMessages;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.errorMessages = errorMessages;
        }

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal();
        };


        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    $scope.title = 'Custom Code Compilation Error Messages';
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }
    }
    appControllers.controller('VR_Analytic_CustomCodeCompilationErrorsEditorController', customCodeCompilationErrorsEditorController);

})(appControllers);