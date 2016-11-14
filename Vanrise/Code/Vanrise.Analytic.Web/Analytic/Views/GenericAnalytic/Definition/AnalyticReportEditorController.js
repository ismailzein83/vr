(function (appControllers) {

    "use strict";

    AnalyticReportEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticReportAPIService', 'VR_Sec_MenuAPIService', 'Analytic_AnalyticService','VR_Analytic_AccessTypeEnum'];

    function AnalyticReportEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticReportAPIService, VR_Sec_MenuAPIService, Analytic_AnalyticService, VR_Analytic_AccessTypeEnum) {

        var isEditMode;
        var analyticReportEntity;
        var configId;
        var analyticReportId;

        var editorDirectiveAPI;
        var editorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                configId = parameters.configId;
                analyticReportId = parameters.analyticReportId;
            }
         
            isEditMode = (analyticReportId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.accessTypes = UtilsService.getArrayEnum(VR_Analytic_AccessTypeEnum);
            $scope.scopeModel.onEditorDirectiveReady = function (api) {
                editorDirectiveAPI = api;
                editorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveAnalyticReport = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
       
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                UtilsService.waitMultipleAsyncOperations([getAnalyticReport, getAnalyticConfigTypes]).then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                getAnalyticConfigTypes().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadEditorDirective]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                if (isEditMode && analyticReportEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(analyticReportEntity.Name, 'Analytic Report Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Analytic Report Editor');
            }

            function loadStaticData() {
                if (analyticReportEntity != undefined) {
                    $scope.scopeModel.reportName = analyticReportEntity.Name;
                    $scope.scopeModel.selectedAccessType = UtilsService.getItemByVal($scope.scopeModel.accessTypes, analyticReportEntity.AccessType, "value");
                }
            } 

            function loadEditorDirective() {
                var loadEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorReadyDeferred.promise.then(function () {
                    var payLoad;
                    if (analyticReportEntity != undefined && analyticReportEntity.Settings != undefined) {
                        payLoad = {
                            reportSettings: analyticReportEntity.Settings
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(editorDirectiveAPI, payLoad, loadEditorDirectivePromiseDeferred);
                });
                return loadEditorDirectivePromiseDeferred.promise;
            }

            function getAnalyticReport() {
                return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportById(analyticReportId).then(function (analyticReportEntityObj) {               
                    analyticReportEntity = analyticReportEntityObj;
                });
            }

        }

        function getAnalyticConfigTypes()
        {
            return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportConfigTypes().then(function (response) {
                $scope.scopeModel.selectedAnalyticReportConfig = UtilsService.getItemByVal(response, configId, "ExtensionConfigurationId");
            });
        }

        function buildAnalyticReportObjectFromScope() {
            
            var analyticReportSettings = editorDirectiveAPI != undefined ? editorDirectiveAPI.getData() : undefined;
            if (analyticReportSettings != undefined)
            {
                analyticReportSettings.ConfigId = $scope.scopeModel.selectedAnalyticReportConfig.ExtensionConfigurationId;
            }
        
            var analyticReport = {
                AnalyticReportId:analyticReportId,
                AccessType: $scope.scopeModel.selectedAccessType !=undefined?$scope.scopeModel.selectedAccessType.value:undefined,
                Name: $scope.scopeModel.reportName,
                Settings: analyticReportSettings,
            };
            return analyticReport;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            var analyticReportEntityObj = buildAnalyticReportObjectFromScope();
            return VR_Analytic_AnalyticReportAPIService.AddAnalyticReport(analyticReportEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Analytic Report', response, 'Name')) {
                    if ($scope.onAnalyticReportAdded != undefined) {
                        $scope.onAnalyticReportAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function update() {
            $scope.scopeModel.isLoading = true;
            var analyticReportEntityObj = buildAnalyticReportObjectFromScope();
            return VR_Analytic_AnalyticReportAPIService.UpdateAnalyticReport(analyticReportEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Analytic Report', response, 'Name')) {
                    if ($scope.onAnalyticReportUpdated != undefined) {
                        $scope.onAnalyticReportUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_Analytic_AnalyticReportEditorController', AnalyticReportEditorController);
})(appControllers);
