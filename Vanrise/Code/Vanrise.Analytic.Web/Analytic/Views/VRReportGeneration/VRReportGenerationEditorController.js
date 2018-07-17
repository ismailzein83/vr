//(function (appControllers) {

//    "use strict";

//    DataAnalysisDefinitionEditorController.$inject = ['$scope', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

//    function DataAnalysisDefinitionEditorController($scope, VR_Analytic_DataAnalysisDefinitionAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

//        var isEditMode;

//        var dataAnalysisDefinitionId;
//        var dataAnalysisDefinitionEntity;

//        var settingsDirectiveAPI;
//        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

//        loadParameters();
//        defineScope();
//        load();


//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined && parameters != null) {
//                dataAnalysisDefinitionId = parameters.dataAnalysisDefinitionId;
//            }

//            isEditMode = (dataAnalysisDefinitionId != undefined);
//        }
//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
//                settingsDirectiveAPI = api;
//                settingsDirectiveReadyDeferred.resolve();
//            };

//            $scope.scopeModel.save = function () {
//                if (isEditMode) {
//                    return update();
//                }
//                else {
//                    return insert();
//                }
//            };
//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal()
//            };
//        }
//        function load() {
//            $scope.scopeModel.isLoading = true;

//            if (isEditMode) {
//                GetDataAnalysisDefinition().then(function () {
//                    loadAllControls();
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                    $scope.scopeModel.isLoading = false;
//                });
//            }
//            else {
//                loadAllControls();
//            }
//        }

//        function GetDataAnalysisDefinition() {
//            return VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(dataAnalysisDefinitionId).then(function (response) {
//                dataAnalysisDefinitionEntity = response;
//            });
//        }

//        function loadAllControls() {
//            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });

//            function setTitle() {
//                if (isEditMode) {
//                    var dataAnalysisDefinitionName = (dataAnalysisDefinitionEntity != undefined) ? dataAnalysisDefinitionEntity.Name : null;
//                    $scope.title = UtilsService.buildTitleForUpdateEditor(dataAnalysisDefinitionName, 'Data Analysis Definition');
//                }
//                else {
//                    $scope.title = UtilsService.buildTitleForAddEditor('Data Analysis Definition');
//                }
//            }
//            function loadStaticData() {
//                if (dataAnalysisDefinitionEntity == undefined)
//                    return;
//                $scope.scopeModel.name = dataAnalysisDefinitionEntity.Name;
//            }
//            function loadSettingsDirective() {
//                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

//                settingsDirectiveReadyDeferred.promise.then(function () {
//                    var settingsDirectivePayload;
//                    if (dataAnalysisDefinitionEntity != undefined) {
//                        settingsDirectivePayload = { dataAnalysisDefinitionSettings: dataAnalysisDefinitionEntity.Settings };
//                    }
//                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
//                });

//                return settingsDirectiveLoadDeferred.promise;
//            }
//        }

//        function insert() {
//            $scope.scopeModel.isLoading = true;
//            return VR_Analytic_DataAnalysisDefinitionAPIService.AddDataAnalysisDefinition(buildDataAnalysisDefinitionObjFromScope()).then(function (response) {
//                if (VRNotificationService.notifyOnItemAdded('DataAnalysisDefinition', response, 'Name')) {
//                    if ($scope.onDataAnalysisDefinitionAdded != undefined)
//                        $scope.onDataAnalysisDefinitionAdded(response.InsertedObject);
//                    $scope.modalContext.closeModal();
//                }
//            }).catch(function (error) {
//                VRNotificationService.notifyException(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }
//        function update() {
//            $scope.scopeModel.isLoading = true;
//            return VR_Analytic_DataAnalysisDefinitionAPIService.UpdateDataAnalysisDefinition(buildDataAnalysisDefinitionObjFromScope()).then(function (response) {
//                if (VRNotificationService.notifyOnItemUpdated('DataAnalysisDefinition', response, 'Name')) {
//                    if ($scope.onDataAnalysisDefinitionUpdated != undefined) {
//                        $scope.onDataAnalysisDefinitionUpdated(response.UpdatedObject);
//                    }
//                    $scope.modalContext.closeModal();
//                }
//            }).catch(function (error) {
//                VRNotificationService.notifyException(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }

//        function buildDataAnalysisDefinitionObjFromScope() {
//            var dataAnalysisDefinitionSettings = settingsDirectiveAPI.getData();

//            return {
//                DataAnalysisDefinitionId: dataAnalysisDefinitionEntity != undefined ? dataAnalysisDefinitionEntity.DataAnalysisDefinitionId : undefined,
//                Name: $scope.scopeModel.name,
//                Settings: dataAnalysisDefinitionSettings
//            };
//        }
//    }

//    appControllers.controller('VR_Analytic_DataAnalysisDefinitionController', DataAnalysisDefinitionEditorController);

//})(appControllers);