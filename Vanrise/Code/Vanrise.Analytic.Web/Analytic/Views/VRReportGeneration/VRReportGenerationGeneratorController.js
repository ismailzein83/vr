(function (appControllers) {

    "use strict";
    vRReportGenerationGeneratorController.$inject = ['$scope', 'VR_Analytic_ReportGenerationAPIService', 'VR_Analytic_ReportGenerationActionService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vRReportGenerationGeneratorController($scope, VR_Analytic_ReportGenerationAPIService, VR_Analytic_ReportGenerationActionService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var reportId;
        var currentReportGenerationInfo;
        var vRReportGenerationEntity;
        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                reportId = parameters.reportId;
                currentReportGenerationInfo = parameters.currentReportGenerationInfo;
            }
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.runtimeEditor;

            $scope.scopeModel.onRuntimeEditorReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.generate = function () {
                $scope.scopeModel.isLoading = true;
                if (vRReportGenerationEntity != undefined || vRReportGenerationEntity.Settings != undefined || vRReportGenerationEntity.Settings.ReportAction != undefined) {
                    var payload = {
                        vRReportGeneration: vRReportGenerationEntity,
                        runtimeFilter: runtimeEditorAPI!=undefined ?runtimeEditorAPI.getData():undefined
                    };
                    var actionTypeName = vRReportGenerationEntity.Settings.ReportAction.ActionTypeName;
                    var actionType = VR_Analytic_ReportGenerationActionService.getActionTypeIfExistByName(actionTypeName);
                    if (actionType != undefined) {

                        var promise = actionType.ExecuteAction(payload);
                        if (promise != undefined) {
                            promise.then(function (response) {
                                $scope.scopeModel.close();
                            }).catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            }).finally(function () {
                                $scope.scopeModel.isLoading = false;
                            });
                        } else {
                            $scope.scopeModel.isLoading = false;
                        }
                    }
                }
                else {
                    $scope.scopeModel.isLoading = false;
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (reportId != undefined && currentReportGenerationInfo==undefined) {
                getVRReportGeneration().then(function () {
                    loadAllControls().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                vRReportGenerationEntity = currentReportGenerationInfo;
                vRReportGenerationEntity.Settings.ReportAction.ActionTypeName = "DownloadFile";
                if (vRReportGenerationEntity.Settings != undefined && vRReportGenerationEntity.Settings.Filter != undefined && vRReportGenerationEntity.Settings.Filter.ConfigId == "38974659-fb26-415e-82bc-2895e1d09238") {
                    $scope.scopeModel.runtimeEditor = "vr-analytic-reportgeneration-filter-standardruntime";
                }
                loadAllControls().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

        }

        function getVRReportGeneration() {
            return VR_Analytic_ReportGenerationAPIService.GetVRReportGeneration(reportId).then(function (response) {
                vRReportGenerationEntity = response;
                if (response.Settings != undefined && response.Settings.Filter != undefined) {
                    $scope.scopeModel.runtimeEditor = response.Settings.Filter.RuntimeEditor;
                }
            });
        }

        function loadAllControls() {
            var promises = [setTitle];
            function loadRuntimeEditor() {
                var loadRuntimeEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                runtimeEditorReadyDeferred.promise.then(function () {

                    var payLoad = undefined;
                    VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, payLoad, loadRuntimeEditorPromiseDeferred);
                });
                return loadRuntimeEditorPromiseDeferred.promise;
            }
            if ($scope.scopeModel.runtimeEditor != undefined) { promises.push(loadRuntimeEditor); }
            function setTitle() {
                $scope.title = "Generate " + vRReportGenerationEntity.Name;
            }

            return UtilsService.waitMultipleAsyncOperations(promises)
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });

        }


    }
    appControllers.controller('VR_Analytic_VRReportGenerationGeneratorController', vRReportGenerationGeneratorController);
})(appControllers);