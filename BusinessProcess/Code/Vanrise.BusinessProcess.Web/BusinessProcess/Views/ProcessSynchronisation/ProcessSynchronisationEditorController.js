(function (appControllers) {

    "use strict";

    ProcessSynchronisationEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_ProcessSynchronisationAPIService', 'BusinessProcess_ProcessSynchronisationService'];

    function ProcessSynchronisationEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_ProcessSynchronisationAPIService, BusinessProcess_ProcessSynchronisationService) {

        var isEditMode;

        var processSynchronisationEntity;
        var settings;
        var processSynchronisationId;

        var firstProcessSynchronisationGroupGridAPI;
        var firstPocessSynchronisationGroupGridReadyDeferred = UtilsService.createPromiseDeferred();

        var secondProcessSynchronisationGroupGridAPI;
        var secondPocessSynchronisationGroupGridReadyDeferred = UtilsService.createPromiseDeferred();

        var executionFlowDefinitionSelectorAPI;
        var executionFlowDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                processSynchronisationId = parameters.processSynchronisationId;
            }

            isEditMode = (processSynchronisationId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFirstProcessSynchronisationGroupGridReady = function (api) {
                firstProcessSynchronisationGroupGridAPI = api;
                firstPocessSynchronisationGroupGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onSecondProcessSynchronisationGroupGridReady = function (api) {
                secondProcessSynchronisationGroupGridAPI = api;
                secondPocessSynchronisationGroupGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionSelectorAPI = api;
                executionFlowDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveProcessSynchronisation = function () {
                if (isEditMode)
                    updateProcessSynchronisation();
                else
                    addProcessSynchronisation();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.validateSecondGroup = function () {
                if (secondProcessSynchronisationGroupGridAPI != undefined && executionFlowDefinitionSelectorAPI != undefined) {
                    var gridData = secondProcessSynchronisationGroupGridAPI.getData();
                    var selectedExecutionFlow = executionFlowDefinitionSelectorAPI.getSelectedIds();

                    if ((gridData == undefined || gridData.length == 0) && selectedExecutionFlow == undefined)
                        return 'Either Add BP Definition or Select Execution Defintion';
                }
                return;
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getProcessSynchronisation(processSynchronisationId).then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getProcessSynchronisation(processSynchronisationId) {
            return BusinessProcess_ProcessSynchronisationAPIService.GetProcessSynchronisation(processSynchronisationId).then(function (response) {
                if (response != undefined) {
                    processSynchronisationEntity = response;
                    settings = processSynchronisationEntity.Settings;
                }
            });
        }

        function loadAllControls() {
            function setTitle() {
                if (processSynchronisationEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(processSynchronisationEntity.Name, 'Process Synchronisation');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Process Synchronisation');
            }

            function loadStaticData() {
                if (processSynchronisationEntity != undefined)
                    $scope.scopeModel.name = processSynchronisationEntity.Name;
            }

            function loadFirstProcessSynchronisationGroupGrid() {
                var firstProcessSynchronisationGroupGridLoadDeferred = UtilsService.createPromiseDeferred();

                firstPocessSynchronisationGroupGridReadyDeferred.promise.then(function () {
                    var firstProcessSynchronisationGroupGridPayload = { shouldAddBPDefinition: true };
                    if (settings != undefined && settings.FirstProcessSynchronisationGroup != undefined)
                        firstProcessSynchronisationGroupGridPayload.processSynchronisationItems = settings.FirstProcessSynchronisationGroup.BPSynchronisationItems;

                    VRUIUtilsService.callDirectiveLoad(firstProcessSynchronisationGroupGridAPI, firstProcessSynchronisationGroupGridPayload, firstProcessSynchronisationGroupGridLoadDeferred);
                });

                return firstProcessSynchronisationGroupGridLoadDeferred.promise;
            }

            function loadSecondProcessSynchronisationGroupGrid() {
                var secondProcessSynchronisationGroupGridLoadDeferred = UtilsService.createPromiseDeferred();

                secondPocessSynchronisationGroupGridReadyDeferred.promise.then(function () {
                    var secondProcessSynchronisationGroupGridPayload = { shouldAddBPDefinition: false };
                    if (settings != undefined && settings.SecondProcessSynchronisationGroup != undefined)
                        secondProcessSynchronisationGroupGridPayload.processSynchronisationItems = settings.SecondProcessSynchronisationGroup.BPSynchronisationItems;

                    VRUIUtilsService.callDirectiveLoad(secondProcessSynchronisationGroupGridAPI, secondProcessSynchronisationGroupGridPayload, secondProcessSynchronisationGroupGridLoadDeferred);
                });

                return secondProcessSynchronisationGroupGridLoadDeferred.promise;
            }

            function loadExecutionFlowDefinitionSelector() {
                var executionFlowDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                executionFlowDefinitionSelectorReadyDeferred.promise.then(function () {
                    var executionFlowDefinitionSelectorPayload;
                    var selectedExecutionFlows = [];

                    if (settings != undefined && settings.SecondProcessSynchronisationGroup != undefined && settings.SecondProcessSynchronisationGroup.ExecutionFlowSynchronisationItems != undefined) {
                        for (var i = 0; i < settings.SecondProcessSynchronisationGroup.ExecutionFlowSynchronisationItems.length; i++) {
                            var currentExecutionFlowId = settings.SecondProcessSynchronisationGroup.ExecutionFlowSynchronisationItems[i].ExecutionFlowDefinitionId;
                            selectedExecutionFlows.push(currentExecutionFlowId);
                        }

                        executionFlowDefinitionSelectorPayload = { selectedIds: selectedExecutionFlows };
                    }
                    VRUIUtilsService.callDirectiveLoad(executionFlowDefinitionSelectorAPI, executionFlowDefinitionSelectorPayload, executionFlowDefinitionSelectorLoadDeferred);
                });

                return executionFlowDefinitionSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadFirstProcessSynchronisationGroupGrid, loadSecondProcessSynchronisationGroupGrid, loadExecutionFlowDefinitionSelector]).then(function () {

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function addProcessSynchronisation() {
            $scope.scopeModel.isLoading = true;
            return BusinessProcess_ProcessSynchronisationAPIService.AddProcessSynchronisation(buildProcessSynchronisationObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('ProcessSynchronisation', response, 'Name')) {
                    if ($scope.onProcessSynchronisationAdded != undefined) {
                        $scope.onProcessSynchronisationAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateProcessSynchronisation() {
            $scope.scopeModel.isLoading = true;
            return BusinessProcess_ProcessSynchronisationAPIService.UpdateProcessSynchronisation(buildProcessSynchronisationObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('ProcessSynchronisation', response, 'Name')) {
                    if ($scope.onProcessSynchronisationUpdated != undefined) {
                        $scope.onProcessSynchronisationUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildProcessSynchronisationObjFromScope() {
            var firstProcessSynchronisatinObj = {
                BPSynchronisationItems: firstProcessSynchronisationGroupGridAPI.getData()
            };

            var executionFlowSynchronisationItems = undefined;

            var selectedExecutionFlowIds = executionFlowDefinitionSelectorAPI.getSelectedIds();
            if (selectedExecutionFlowIds != undefined) {
                executionFlowSynchronisationItems = [];
                executionFlowSynchronisationItems.push({ ExecutionFlowDefinitionId: selectedExecutionFlowIds });
            }
            var secondProcessSynchronisatinObj = {
                BPSynchronisationItems: secondProcessSynchronisationGroupGridAPI.getData(),
                ExecutionFlowSynchronisationItems: executionFlowSynchronisationItems
            };

            var processSynchronisationObj = {
                Name: $scope.scopeModel.name,
                Settings: {
                    FirstProcessSynchronisationGroup: firstProcessSynchronisatinObj,
                    SecondProcessSynchronisationGroup: secondProcessSynchronisatinObj
                }
            };

            if (isEditMode) {
                processSynchronisationObj.ProcessSynchronisationId = processSynchronisationId;
            }

            return processSynchronisationObj;
        }
    }

    appControllers.controller('BusinessProcess_ProcessSynchronisationEditorController', ProcessSynchronisationEditorController);
})(appControllers);