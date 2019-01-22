(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptQueriesDisplayerController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function generatedScriptQueriesDisplayerController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var queries;

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                queries = parameters.queries;
            }

        }

        function defineScope() {

            $scope.scopeModel.saveGeneratedScriptDesign = function () {
                if (isEditMode)
                    return updateGeneratedScriptDesign();
                else
                    return insertGeneratedScriptDesign();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.Queries = queries;
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {

            if (isEditMode) {
                connectionStringSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                schemaSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                tableSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }


            function setTitle() {
                $scope.title = "Queries";
            }

            function loadStaticData() {

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

    }
    appControllers.controller('VR_Devtools_GeneratedScriptQueriesDisplayerController', generatedScriptQueriesDisplayerController);
})(appControllers);