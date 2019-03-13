(function (appControllers) {

    "use strict";

    AssignTaskEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_BPTaskTypeAPIService', 'VR_GenericData_DataRecordTypeAPIService'];

    function AssignTaskEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService, BusinessProcess_BPTaskTypeAPIService, VR_GenericData_DataRecordTypeAPIService) {

        var taskTypeId;
        var taskTitle;
        var displayName;
        var taskAssignees;
        var filter;
        var inputItems = [];
        var outputItems = [];
        var context;
        var isNew;

        var bpTsakTypeEntity;

        var bPTaskTypeSelectorAPI;
        var bPTaskTypeSelectorReadyDeffered = UtilsService.createPromiseDeferred();

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
                context = parameters.context;
                isNew = parameters.isNew;
            }
        }

        function defineScope() {
            $scope.scopeModel = { };
            $scope.scopeModel.taskTypes = [];
            $scope.scopeModel.inputItems = [];
            $scope.scopeModel.outputItems = [];
            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
            $scope.scopeModel.isBPTaskTypeLoading = true;

            $scope.modalContext.onModalHide = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
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
                            var recordTypeId = bpTsakTypeEntity.Settings.RecordTypeId;
                            loadColumns(recordTypeId);
                        }
                    });
                }
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
            function setTitle() {
                $scope.title = "Edit Assign Task";
            }

            function loadStaticData() {
                $scope.scopeModel.taskTitle = taskTitle;
                $scope.scopeModel.displayName = displayName;
            }

            function loadBPTaskTypeSelector() {
                BusinessProcess_BPTaskTypeAPIService.GetBPTaskTypesInfo(filter).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var bpTasktype = response[i];
                            $scope.scopeModel.taskTypes.push(bpTasktype)
                        }
                    }
                    if (taskTypeId != undefined) {
                        BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(taskTypeId).then(function (response) {
                            if (response != undefined) {
                                var taskTypeEntity = response;
                                $scope.scopeModel.selectedTaskType = taskTypeEntity;
                            }
                        });
                       
                    }
                    $scope.scopeModel.isBPTaskTypeLoading = false;
                });
            }

            function loadGrids(recordTypeId) {
                loadColumns(recordTypeId);
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBPTaskTypeSelector, loadGrids]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadColumns(recordTypeId) {
            if (recordTypeId == undefined && bpTsakTypeEntity != undefined) {
                recordTypeId = bpTsakTypeEntity.Settings.RecordTypeId;
            }
            if (recordTypeId != undefined) {
                VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(recordTypeId).then(function (response) {
                    if (response != undefined) {
                        $scope.scopeModel.inputItems = [];
                        $scope.scopeModel.outputItems = [];
                        for (var i = 0; i < response.Fields.length; i++) {
                            var item = response.Fields[i];

                            var gridInputItem = {
                                id: $scope.scopeModel.inputItems.length + 1,
                            };

                            var gridOutputItem = {
                                id: $scope.scopeModel.outputItems.length + 1,
                            };

                            gridInputItem.fieldName = item.Title;
                            gridOutputItem.fieldName = item.Title;
                            if (inputItems != undefined && inputItems.length > 0 && outputItems != undefined && outputItems.length>0) {
                                gridInputItem.inputValue = inputItems[i].Value;
                                gridOutputItem.outputTo = outputItems[i].To;
                            }
                            $scope.scopeModel.inputItems.push(gridInputItem);
                            $scope.scopeModel.outputItems.push(gridOutputItem);
                        }
                    }
                });
            }
        }

        function updateActivity() {
            $scope.scopeModel.isLoading = true;
            var updatedObject = {
                taskTypeId: $scope.scopeModel.selectedTaskType.BPTaskTypeId,
                taskTitle: $scope.scopeModel.taskTitle,
                displayName: $scope.scopeModel.displayName,
                taskAssignees: {},
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
                columns.push({
                    FieldName: column.fieldName,
                    Value: column.inputValue,
                });
            }
            return columns;
        }

        function getOutputColumns() {
            var columns = [];
            for (var i = 0; i < $scope.scopeModel.outputItems.length; i++) {
                var column = $scope.scopeModel.outputItems[i];
                columns.push({
                    FieldName: column.fieldName,
                    To: column.outputTo,
                });
            }
            return columns;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowActivityAssignTaskController', AssignTaskEditorController);
})(appControllers);