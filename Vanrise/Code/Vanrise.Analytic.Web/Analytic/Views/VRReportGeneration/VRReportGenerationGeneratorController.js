(function (appControllers) {

    "use strict";
    vRReportGenerationGeneratorController.$inject = ['$scope', 'VR_Analytic_ReportGenerationAPIService', 'VR_Analytic_ReportGenerationActionService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vRReportGenerationGeneratorController($scope, VR_Analytic_ReportGenerationAPIService, VR_Analytic_ReportGenerationActionService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var reportId;
        var vRReportGenerationEntity;



        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                reportId = parameters.reportId;
            }
        };

        function defineScope() {

            $scope.scopeModel = {};


            $scope.scopeModel.generate = function () {
                $scope.scopeModel.isLoading = true;
                if (vRReportGenerationEntity != undefined || vRReportGenerationEntity.Settings != undefined || vRReportGenerationEntity.Settings.ReportAction != undefined) {
                    var payload = { vRReportGeneration: vRReportGenerationEntity };
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
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            getVRReportGeneration().then(function () {
                loadAllControls().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });


        };

        function getVRReportGeneration() {
            return VR_Analytic_ReportGenerationAPIService.GetVRReportGeneration(reportId).then(function (response) {
                vRReportGenerationEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Generate " + vRReportGenerationEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });

        };


    };
    appControllers.controller('VR_Analytic_VRReportGenerationGeneratorController', vRReportGenerationGeneratorController);
})(appControllers);