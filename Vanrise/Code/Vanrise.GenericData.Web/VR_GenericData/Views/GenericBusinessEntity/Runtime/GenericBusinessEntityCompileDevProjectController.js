(function (appControllers) {

    "use strict";

    GenericBusinessEntityCompileDevProjectController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Common_DevProjectAPIService', 'VR_GenericData_GenericBEDefinitionActionCompileBuildOptionEnum','VRCommon_CompilationService'];

    function GenericBusinessEntityCompileDevProjectController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Common_DevProjectAPIService, VR_GenericData_GenericBEDefinitionActionCompileBuildOptionEnum, VRCommon_CompilationService) {
        var devProjectId;
        var buildOptionSelectorAPI;
        var buildOptionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                devProjectId = parameters.devProjectId;
            }
        }

        function defineScope() {

            $scope.scopeModel.onBuildOptionSelectorReady = function (api) {
                buildOptionSelectorAPI = api;
                buildOptionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.compile = function () {
                return tryCompileDevproject();
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
                $scope.title = "Compile DevProject";
            }
            return UtilsService.waitPromiseNode({
                promises: [loadBuildOptionSelector()],
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function tryCompileDevproject() {
            $scope.scopeModel.isLoading = true;

            return VR_Common_DevProjectAPIService.TryCompileDevProject(devProjectId, buildOptionSelectorAPI.getSelectedIds())
                .then(function (response) {
                    if (response.Result) {
                        VRNotificationService.showSuccess("Project compiled successfully.");
                        $scope.modalContext.closeModal();
                    }
                    else {
                        VRCommon_CompilationService.tryCompilationResult(response.ErrorMessages);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function loadBuildOptionSelector() {
            var buildOptionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            buildOptionSelectorReadyDeferred.promise.then(function () {
                var buildOptionSelectorPayload = {
                    selectedIds: VR_GenericData_GenericBEDefinitionActionCompileBuildOptionEnum.Build.value
                };
                VRUIUtilsService.callDirectiveLoad(buildOptionSelectorAPI, buildOptionSelectorPayload, buildOptionSelectorLoadDeferred);
            });
            return buildOptionSelectorLoadDeferred.promise;
        }

    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityCompileDevProjectController', GenericBusinessEntityCompileDevProjectController);
})(appControllers);
