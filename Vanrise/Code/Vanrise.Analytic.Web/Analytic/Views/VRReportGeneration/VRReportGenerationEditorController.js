(function (appControllers) {

    "use strict";
    vRReportGenerationEditorController.$inject = ['$scope', 'VR_Analytic_ReportGenerationAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Analytic_AccessTypeEnum'];

    function vRReportGenerationEditorController($scope, VR_Analytic_ReportGenerationAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Analytic_AccessTypeEnum) {

        var isEditMode;
        var reportId;
        var vRReportGenerationEntity;
        var isViewHistoryMode;
        var context;
        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                reportId = parameters.reportId;
                context = parameters.context;
            }
            isEditMode = (reportId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.accessTypes = UtilsService.getArrayEnum(VR_Analytic_AccessTypeEnum);
            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVRReportGeneration().then(function () {
                    loadAllControls().finally(function () {
                        vRReportGenerationEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else if (isViewHistoryMode) {
                getVRReportGenerationHistory().then(function () {
                    loadAllControls().finally(function () {
                        vRReportGenerationEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else
                loadAllControls();
        };

        function getVRReportGenerationHistory() {
            return VR_Analytic_ReportGenerationAPIService.GetVRReportGenerationHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                vRReportGenerationEntity = response;
            });
        }
        function getVRReportGeneration() {
            return VR_Analytic_ReportGenerationAPIService.GetVRReportGeneration(reportId).then(function (response) {
                vRReportGenerationEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && vRReportGenerationEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vRReportGenerationEntity.Name, "VR Report Generation");
                else if (isViewHistoryMode && vRReportGenerationEntity != undefined)
                    $scope.title = "View Report Generation: " + vRReportGenerationEntity.Name;
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("VR Report Generation");
            };

            function loadStaticData() {
                if (vRReportGenerationEntity != undefined)
                {
                    $scope.scopeModel.name = vRReportGenerationEntity.Name;
                    $scope.scopeModel.description = vRReportGenerationEntity.Description;
                    $scope.scopeModel.selectedAccessLevel = UtilsService.getItemByVal($scope.scopeModel.accessTypes, vRReportGenerationEntity.AccessLevel, "value");
                }
            };

            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload; 
                    if (vRReportGenerationEntity != undefined) {
                        settingsDirectivePayload = { Settings: vRReportGenerationEntity.Settings };
                    }
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });

        };

        function buildVRReportGenerationObjectFromScope() {
            var settings = settingsDirectiveAPI.getData();
            var object = {
                ReportId: (reportId != undefined) ? reportId : undefined,
                Name: $scope.scopeModel.name,
                Description:$scope.scopeModel.description,
                    AccessLevel: $scope.scopeModel.selectedAccessLevel !=undefined?$scope.scopeModel.selectedAccessLevel.value: undefined,
                Settings: settings                    
            };
            return object;
        };

        function insert() {

            $scope.scopeModel.isLoading = true;
            var vRReportGenerationObject = buildVRReportGenerationObjectFromScope();
            return VR_Analytic_ReportGenerationAPIService.AddVRReportGeneration(vRReportGenerationObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("VRReportGeneration", response, "Name")) {
                    if ($scope.onVRReportGenerationAdded != undefined) {
                        $scope.onVRReportGenerationAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function update() {
            $scope.scopeModel.isLoading = true;
            var vRReportGenerationObject = buildVRReportGenerationObjectFromScope();
            VR_Analytic_ReportGenerationAPIService.UpdateVRReportGeneration(vRReportGenerationObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("VRReportGeneration", response, "Name")) {
                    if ($scope.onVRReportGenerationUpdated != undefined) {
                        $scope.onVRReportGenerationUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('VR_Analytic_VRReportGenerationEditorController', vRReportGenerationEditorController);
})(appControllers);