(function (appControllers) {

    "use strict";

    AssignTaskEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_BPTaskTypeAPIService', 'VR_GenericData_DataRecordTypeAPIService'];

    function AssignTaskEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService, BusinessProcess_BPTaskTypeAPIService, VR_GenericData_DataRecordTypeAPIService) {

        var taskTypeId;
        var taskTitle;
        var displayName;
        var taskAssignees;
        var inputItems = [];
        var outputItems = [];
        var context;
        var isNew;
        var recordTypeId;

        var bpTsakTypeEntity;

        var bPTaskTypeSelectorAPI;
        var bPTaskTypeSelectorReadyDeffered = UtilsService.createPromiseDeferred();

        var taskAssigneesAPI;
        var taskAssigneesPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var inputGridAPI;
        var inputGridPromiseReadyDefferd = UtilsService.createPromiseDeferred();

        var outputGridAPI;
        var outputGridPromiseReadyDefferd = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);


            if (parameters != undefined && parameters.obj != undefined) {
                taskTypeId = parameters.obj.TaskTypeId;
                taskTitle = parameters.obj.TaskTitle;
                displayName = parameters.obj.DisplayName;
                taskAssignees = parameters.obj.TaskAssignees;
                if (parameters.obj.InputItems != undefined) {
                    for (var i = 0; i < parameters.obj.InputItems.length; i++) {
                        var inputItem = parameters.obj.InputItems[i];
                        inputItems.push(inputItem);
                    }
                }
                if (parameters.obj.OutputItems != undefined) {
                    for (var j = 0; j < parameters.obj.OutputItems.length; j++) {
                        var inputItem = parameters.obj.OutputItems[j];
                        outputItems.push(inputItem);
                    }
                }
                isNew = parameters.isNew;
                context = parameters.context;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.taskTypes = [];
            $scope.scopeModel.inputItems = [];
            $scope.scopeModel.outputItems = [];
            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
            $scope.scopeModel.isBPTaskTypeLoading = true;

            $scope.scopeModel.context = context;

            $scope.modalContext.onModalHide = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
            };

            $scope.scopeModel.onTaskAssigneesSelectorReady = function (api) {
                taskAssigneesAPI = api;
                taskAssigneesPromiseReadyDeffered.resolve();
            };

            $scope.scopeModel.onBPTaskTypeSelectorReady = function (api) {
                bPTaskTypeSelectorAPI = api;
                bPTaskTypeSelectorReadyDeffered.resolve();
            };

            $scope.scopeModel.onBPTaskTypeSelectionChanged = function (taskTypeField) {
                if (taskTypeField != undefined) {
                    BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(taskTypeField.BPTaskTypeId).then(function (response) {
                        bpTsakTypeEntity = response;
                        if (bpTsakTypeEntity != undefined) {
                            recordTypeId = bpTsakTypeEntity.Settings.RecordTypeId;
                            loadColumns(recordTypeId);
                        }
                    });
                }
            };

            $scope.scopeModel.onInputGridReady = function (api) {
                inputGridAPI = api;
                inputGridPromiseReadyDefferd.resolve();
            };

            $scope.scopeModel.onOutputGridReady = function (api) {
                outputGridAPI = api;
                outputGridPromiseReadyDefferd.resolve();
            };

            $scope.scopeModel.saveActivity = function () {
                return updateActivity();
            };

            $scope.scopeModel.close = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            var promises = [];
            function setTitle() {
                $scope.title = "Edit Human Activity";
            }

            function loadStaticData() {
                $scope.scopeModel.taskTitle = taskTitle;
                $scope.scopeModel.displayName = displayName;
            }

            function loadBPTaskTypeSelector() {
                var filter = {
                    Filters: [{
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskTypeSettingsFilter, Vanrise.BusinessProcess.MainExtensions"

                    }]
                };
                var bPTaskTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                bPTaskTypeSelectorReadyDeffered.promise.then(function () {
                    var bpTaskTypePayload = {
                        filter: filter,
                        selectedIds: taskTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(bPTaskTypeSelectorAPI, bpTaskTypePayload, bPTaskTypeSelectorLoadPromiseDeferred);
                });
                return bPTaskTypeSelectorLoadPromiseDeferred.promise;
            }

            function loadTaskAssigneesSelector() {
                var taskAssigneesLoadDeferred = UtilsService.createPromiseDeferred();
                taskAssigneesPromiseReadyDeffered.promise.then(function () {
                    var taskAssigneesPayload = {
                        taskAssignees: taskAssignees,
                        getWorkflowArguments: context.getWorkflowArguments,
                        getParentVariables: context.getParentVariables,
                        isVRWorkflowActivityDisabled: $scope.scopeModel.isVRWorkflowActivityDisabled
                    };
               
                    VRUIUtilsService.callDirectiveLoad(taskAssigneesAPI, taskAssigneesPayload, taskAssigneesLoadDeferred);
                });
                return taskAssigneesLoadDeferred.promise;
            }

            function loadGrids(promises) {
                loadColumns(recordTypeId, promises);
            }


            promises.push(setTitle);
            promises.push(loadStaticData);
            promises.push(loadBPTaskTypeSelector);
            promises.push(loadTaskAssigneesSelector);
            promises.push(loadGrids);
            return UtilsService.waitMultipleAsyncOperations(promises).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadColumns(recordTypeId, promises) {
            if (recordTypeId != undefined) {

                var loadColumnsPromiseDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.isInputGridLoading = true;
                $scope.scopeModel.isOutputGridLoading = true;

                VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(recordTypeId).then(function (response) {
                    if (response != undefined) {
                        inputGridAPI.clearDataSource();
                        outputGridAPI.clearDataSource();

                        for (var i = 0; i < response.Fields.length; i++) {
                            var inputItem = {
                                payload: response.Fields[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            var outputItem = {
                                payload: response.Fields[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };

                            addInputGrid(inputItem);
                            addOutputItem(outputItem);

                            if (promises != undefined) {
                                promises.push(inputItem.loadPromiseDeferred.promise);
                                promises.push(outputItem.loadPromiseDeferred.promise);
                            }
                        }

                        $scope.scopeModel.isInputGridLoading = false;
                        $scope.scopeModel.isOutputGridLoading = false;
                        if (promises != undefined) {
                            promises.push(loadColumnsPromiseDeferred);
                        }
                    }
                });
            }
        }


        function addInputGrid(inputItem) {
            var dataItem = {
                id: $scope.scopeModel.inputItems.length + 1,
                fieldName: inputItem.payload.Name,
            };

            if (inputItems.length > 0) {
                var item = inputItems.find(x => x.FieldName === dataItem.fieldName);
                if (item != undefined) {
                    dataItem.inputValue = item.Value;
                }
            }

            var dataItemPayload = inputItem.payload;

            dataItem.onDirectiveReady = function (api) {
                dataItem.directiveAPI = api;
                inputItem.readyPromiseDeferred.resolve();
            };

            inputItem.readyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, inputItem.loadPromiseDeferred);
            });

            $scope.scopeModel.inputItems.push(dataItem);
        }

        function addOutputItem(outputItem) {
            var dataItem = {
                id: $scope.scopeModel.outputItems.length + 1,
                fieldName: outputItem.payload.Name,
            };

            if (outputItems.length > 0) {
                var item = outputItems.find(y => y.FieldName === dataItem.fieldName);
                if (item != undefined) {
                    dataItem.outputTo = item.To;
                }
            }
            var dataItemPayload = outputItem.payload;

            dataItem.onDirectiveReady = function (api) {
                dataItem.directiveAPI = api;
                outputItem.readyPromiseDeferred.resolve();
            };

            outputItem.readyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, outputItem.loadPromiseDeferred);
            });

            $scope.scopeModel.outputItems.push(dataItem);
        }

        function updateActivity() {
            $scope.scopeModel.isLoading = true;
            var taskAssigneeSettings = taskAssigneesAPI.getData();
            var updatedObject = {
                taskTypeId: $scope.scopeModel.selectedTaskType.BPTaskTypeId,
                taskTitle: $scope.scopeModel.taskTitle,
                displayName: $scope.scopeModel.displayName,
                taskAssignees: { Settings: taskAssigneeSettings},
                inputItems: $scope.scopeModel.inputItems.length > 0 ? getInputColumns() : null,
                outputItems: $scope.scopeModel.outputItems.length > 0 ? getOutputColumns() : null,
            };

            if ($scope.onActivityUpdated != undefined) {
                $scope.onActivityUpdated(updatedObject);
            }

            $scope.scopeModel.isLoading = false;
            isNew = false;
            $scope.modalContext.closeModal();
        }

        function getInputColumns() {
            var columns = [];
            for (var i = 0; i < $scope.scopeModel.inputItems.length; i++) {
                var column = $scope.scopeModel.inputItems[i];
                if (column.inputValue != undefined) {
                    columns.push({
                        FieldName: column.fieldName,
                        Value: column.inputValue,
                    });
                }
            }
            return columns;
        }

        function getOutputColumns() {
            var columns = [];
            for (var i = 0; i < $scope.scopeModel.outputItems.length; i++) {
                var column = $scope.scopeModel.outputItems[i];
                if (column.outputTo != undefined) {
                    columns.push({
                        FieldName: column.fieldName,
                        To: column.outputTo,
                    });
                }
            }
            return columns;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowActivityAssignTaskController', AssignTaskEditorController);
})(appControllers);