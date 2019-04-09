//(function (appControllers) {

//    "use strict";

//    AddBusinessEntityEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_BPTaskTypeAPIService', 'VR_GenericData_DataRecordTypeAPIService'];

//    function AddBusinessEntityEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService, BusinessProcess_BPTaskTypeAPIService, VR_GenericData_DataRecordTypeAPIService) {

//        var taskTypeId;
//        var taskTitle;
//        var displayName;
//        var taskAssignees;
//        var inputItems = [];
//        var outputItems = [];
//        var context;
//        var isNew;
//        var businessEntityDefinitionId;
//        var settings;
//        var beDefinitionSelectorApi;
//        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//        var settingsDirectiveAPI;
//        var settingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

       

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);


//            if (parameters != undefined && parameters.obj != undefined) {
//                displayName = parameters.obj.DisplayName;
//                businessEntityDefinitionId = parameters.obj.EntityDefinitionId;
//                settings = parameters.obj.settings;
//                isNew = parameters.isNew;
//                context = parameters.context;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel = {};
//            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
//            $scope.scopeModel.selectedBusinessEntity;
//            $scope.scopeModel.context = context;

//            $scope.modalContext.onModalHide = function () {
//                if ($scope.remove != undefined && isNew == true) {
//                    $scope.remove();
//                }
//            };

           
//            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
//                beDefinitionSelectorApi = api;
//                beDefinitionSelectorPromiseDeferred.resolve();
//            };

//            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
//                settingsDirectiveAPI = api;
//                settingsReadyPromiseDeferred.resolve();
//                var setLoader = function (value) {
//                    $scope.scopeModel.isLoadingSettingsDirective = value;
//                };
//                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsDirectiveAPI, {
//                    context: context,
//                    businessEntityDefinitionId: $scope.scopeModel.selectedBusinessEntity.BusinessEntityDefinitionId,
//                    settings: settings
//                }, setLoader);
//            };
//            //$scope.scopeModel.onBPTaskTypeSelectionChanged = function (taskTypeField) {
//            //    if (taskTypeField != undefined) {
//            //        BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(taskTypeField.BPTaskTypeId).then(function (response) {
//            //            bpTsakTypeEntity = response;
//            //            if (bpTsakTypeEntity != undefined) {
//            //                recordTypeId = bpTsakTypeEntity.Settings.RecordTypeId;
//            //                loadColumns(recordTypeId);
//            //            }
//            //        });
//            //    }
//            //};

//            $scope.scopeModel.saveActivity = function () {
//                return updateActivity();
//            };

//            $scope.scopeModel.close = function () {
//                if ($scope.remove != undefined && isNew == true) {
//                    $scope.remove();
//                }
//                $scope.modalContext.closeModal();
//            };
//        }

//        function load() {
//            $scope.scopeModel.isLoading = true;
//            loadAllControls();
//        }

//        function loadAllControls() {
//            var promises = [];
//            function setTitle() {
//                $scope.title = "Edit Add Business Entity Activity";
//            }

//            function loadStaticData() {
//                $scope.scopeModel.displayName = displayName;
//            }


//            function loadBusinessEntityDefinitionSelector() {
//                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                beDefinitionSelectorPromiseDeferred.promise.then(function () {
//                    var payloadSelector = {
//                        selectedIds: businessEntityDefinitionId
//                    };
//                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
//                });
//                return businessEntityDefinitionSelectorLoadDeferred.promise;
//            }
        

//            promises.push(setTitle);
//            promises.push(loadStaticData);
//            promises.push(loadBusinessEntityDefinitionSelector);
//            return UtilsService.waitMultipleAsyncOperations(promises).then(function () {
//            }).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }

//        function updateActivity() {
//            $scope.scopeModel.isLoading = true;
//            var updatedObject = {
//                displayName: $scope.scopeModel.displayName,
//                entityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
//                settings: settingsDirectiveAPI.getData()
//            };

//            if ($scope.onActivityUpdated != undefined) {
//                $scope.onActivityUpdated(updatedObject);
//            }

//            $scope.scopeModel.isLoading = false;
//            isNew = false;
//            $scope.modalContext.closeModal();
//        }

//    }

//    appControllers.controller('BusinessProcess_VR_WorkflowActivityAddBusinessEntityController', AddBusinessEntityEditorController);
//})(appControllers);