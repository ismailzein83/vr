﻿(function (appControllers) {

    "use strict";

    BPTechnicalDefinitionEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService'];

    function BPTechnicalDefinitionEditorController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

        var isEditMode;
        var vrWorkflowArguments;
        var validationMessages;

        var businessProcessDefinitionId;
        var businessProcessDefinitionEntity;

        var vrWorkflowSelectorAPI;
        var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var vrWorkflowSelectorSelectionChangedDeferred;

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var startNewInstancePermissionAPI;
        var startNewInstancePermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var scheduleTaskPermissionAPI;
        var scheduleTaskPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var bpInstanceInsertHandlerSettingsAPI;
        var bpInstanceInsertHandlerSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var editorDefinitionAPI;
        var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var vrWorkflowFields;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                businessProcessDefinitionId = parameters.businessProcessDefinitionId;
            }

            isEditMode = (businessProcessDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.loadVRWorklowSelector = isEditMode ? false : true;

            $scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
                vrWorkflowSelectorAPI = api;
                vrWorkflowSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onStartNewInstanceRequiredPermissionReady = function (api) {
                startNewInstancePermissionAPI = api;
                startNewInstancePermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onScheduleTaskRequiredPermissionReady = function (api) {
                scheduleTaskPermissionAPI = api;
                scheduleTaskPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onBPIntanceInsertHandlerSettingsReady = function (api) {
                bpInstanceInsertHandlerSettingsAPI = api;
                bpInstanceInsertHandlerSettingsReadyDeferred.resolve();
            };

            $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                editorDefinitionAPI = api;
                editorDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onVRWorkflowSelectionChanged = function (selectedVRWorkflow) {
                validationMessages = undefined;
                if (selectedVRWorkflow != undefined) {

                    if (vrWorkflowSelectorSelectionChangedDeferred != undefined) {
                        vrWorkflowSelectorSelectionChangedDeferred.resolve();
                    }
                    else {
                        $scope.scopeModel.isLoadingArguments = true;
                        getVRWorkflowArguments(selectedVRWorkflow.VRWorkflowId).then(function () {
                            $scope.scopeModel.processTitle = "";
                            $scope.scopeModel.isLoadingArguments = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            $scope.scopeModel.isLoadingArguments = false;
                        });
                        getVRWorkflowInputArgumentFields(selectedVRWorkflow.VRWorkflowId).then(function () {
                            if (editorDefinitionAPI != undefined) {
                                var setEditorLoader = function (value) { $scope.scopeModel.isLoadingEditor = value; };
                                var editorPayload = {
                                    context: getContext()
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorDefinitionAPI, editorPayload, setEditorLoader);
                            }
                        });
                    }
                }
            };

            $scope.scopeModel.getVRWorkflowArguments = function () {
                return vrWorkflowArguments;
            };

            $scope.saveBPDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onCompileClick = function () {
                if ($scope.scopeModel.title == undefined || $scope.scopeModel.title == '') {
                    VRNotificationService.showWarning("Title is required");
                    return;
                }

                var vrWorkflowId = $scope.scopeModel.loadVRWorklowSelector ? vrWorkflowSelectorAPI.getSelectedIds() : null;
                if (vrWorkflowId == null) {
                    VRNotificationService.showWarning("Workflow is required");
                    return;
                }

                $scope.scopeModel.isLoadingArguments = true;

                BusinessProcess_BPDefinitionAPIService.TryCompileProcessTitle(buildBPEntityObjFromScope()).then(function (response) {
                    $scope.scopeModel.isLoadingArguments = false;

                    validationMessages = response.ErrorMessages;
                    if (validationMessages == undefined || validationMessages.length == 0)
                        VRNotificationService.showSuccess("Custom Code compiled successfully.");
                });
            };

            $scope.scopeModel.isTitleValid = function () {
                if (validationMessages == undefined || validationMessages.length == 0)
                    return;
                else {
                    var errorMessage = '';
                    for (var i = 0; i < validationMessages.length; i++) {
                        errorMessage += (i + 1) + ') ' + validationMessages[i] + '\n';
                    }
                    return errorMessage;
                }
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getBusinessProcessDefinition().then(function () {
                    if (businessProcessDefinitionEntity.VRWorkflowId != undefined) {
                        $scope.scopeModel.loadVRWorklowSelector = true;
                    }
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getBusinessProcessDefinition() {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(businessProcessDefinitionId).then(function (response) {
                businessProcessDefinitionEntity = response;
            });
        }

        function getVRWorkflowArguments(vrWorkflowId) {
            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowArguments(vrWorkflowId).then(function (response) {
                vrWorkflowArguments = response;
            });
        }

        function loadEditorDefinitionDirective() {
            var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            editorDefinitionReadyPromiseDeferred.promise.then(function () {
                var editorPayload = {
                    settings: businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined ? businessProcessDefinitionEntity.Configuration.EditorSettings : undefined,
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
            });
            return loadEditorDefinitionDirectivePromiseDeferred.promise;
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(businessProcessDefinitionEntity.Title, 'Business Process Editor');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Business Process Editor');
                }
            }
            function loadStaticData() {
                if (!isEditMode)
                    return;

                $scope.scopeModel.title = businessProcessDefinitionEntity.Title;
                $scope.scopeModel.MaxConcurrentWorkflows = businessProcessDefinitionEntity.Configuration.MaxConcurrentWorkflows;
                $scope.scopeModel.NotVisibleInManagementScreen = businessProcessDefinitionEntity.Configuration.NotVisibleInManagementScreen;
                $scope.scopeModel.processTitle = businessProcessDefinitionEntity.Configuration.ProcessTitle;
            }
            function loadVRWorkflowSelector() {
                if (!$scope.scopeModel.loadVRWorklowSelector)
                    return;

                var vrWorkflowLoadDeferred = UtilsService.createPromiseDeferred();

                vrWorkflowSelectorReadyDeferred.promise.then(function () {

                    var vrWorkflowSelectorPayload;
                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.VRWorkflowId) {
                        vrWorkflowSelectorPayload = {
                            selectedIds: businessProcessDefinitionEntity.VRWorkflowId
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowLoadDeferred);
                });

                return vrWorkflowLoadDeferred.promise;
            }
            function loadViewRequiredPermission() {
                var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                viewPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.View != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.View
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                });

                return viewPermissionLoadDeferred.promise;
            }
            function loadStartNewInstanceRequiredPermission() {
                var startNewInstancePermissionLoadDeferred = UtilsService.createPromiseDeferred();

                startNewInstancePermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.StartNewInstance != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.StartNewInstance
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(startNewInstancePermissionAPI, payload, startNewInstancePermissionLoadDeferred);
                });

                return startNewInstancePermissionLoadDeferred.promise;
            }
            function loadScheduleTaskRequiredPermission() {
                var scheduleTaskPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                scheduleTaskPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.ScheduleTask != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.ScheduleTask
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(scheduleTaskPermissionAPI, payload, scheduleTaskPermissionLoadDeferred);
                });

                return scheduleTaskPermissionLoadDeferred.promise;
            }
            function loadBPInstanceInsertHandlerSettings() {
                var bpInstanceInsertHandlerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                bpInstanceInsertHandlerSettingsReadyDeferred.promise.then(function () {

                    var payload;
                    if (businessProcessDefinitionEntity && businessProcessDefinitionEntity.Configuration && businessProcessDefinitionEntity.Configuration.BPInstanceInsertHandler) {
                        payload = {
                            bpInstanceInsertHandler: businessProcessDefinitionEntity.Configuration.BPInstanceInsertHandler
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(bpInstanceInsertHandlerSettingsAPI, payload, bpInstanceInsertHandlerSettingsLoadPromiseDeferred);
                });

                return bpInstanceInsertHandlerSettingsLoadPromiseDeferred.promise;
            }
       
            function GetVRWorkflowArguments() {
                if (!isEditMode || businessProcessDefinitionEntity.VRWorkflowId == undefined)
                    return;

                var getVRWorkflowArgumentsLoadDeferred = UtilsService.createPromiseDeferred();
                vrWorkflowSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                getVRWorkflowArguments(businessProcessDefinitionEntity.VRWorkflowId).then(function () {
                    getVRWorkflowArgumentsLoadDeferred.resolve();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    getVRWorkflowArgumentsLoadDeferred.reject();
                    });

                vrWorkflowSelectorSelectionChangedDeferred.promise.then(function () {
                    vrWorkflowSelectorSelectionChangedDeferred = undefined;
                });

                return getVRWorkflowArgumentsLoadDeferred.promise;
            }
            var operations = [setTitle, loadStaticData, loadVRWorkflowSelector, GetVRWorkflowArguments, loadViewRequiredPermission, loadStartNewInstanceRequiredPermission,
                loadScheduleTaskRequiredPermission, loadBPInstanceInsertHandlerSettings];
            if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.VRWorkflowId != undefined) {
                operations.push(getVRWorkflowInputArgumentFields);
                operations.push(loadEditorDefinitionDirective);
            }
            return UtilsService.waitMultipleAsyncOperations(operations).then(function () {
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function getVRWorkflowInputArgumentFields(vrWorkflowId) {
            if (vrWorkflowId == undefined)
                vrWorkflowId = businessProcessDefinitionEntity.VRWorkflowId;
            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowInputArgumentFields(vrWorkflowId).then(function (response) {
                vrWorkflowFields = response;
            });
        }
        function insert() {
            $scope.scopeModel.isLoading = true;
            validationMessages = undefined;

            var insertBPDefintionPromiseDeferred = UtilsService.createPromiseDeferred();

            BusinessProcess_BPDefinitionAPIService.AddBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('BP Definition', response, 'Title')) {
                    if ($scope.onBPDefenitionAdded != undefined)
                        $scope.onBPDefenitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();

                    insertBPDefintionPromiseDeferred.resolve();
                }
                else {
                    validationMessages = response.ValidationMessages;
                    insertBPDefintionPromiseDeferred.reject();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                insertBPDefintionPromiseDeferred.reject();
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            return insertBPDefintionPromiseDeferred.promise;
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            validationMessages = undefined;

            var updateBPDefintionPromiseDeferred = UtilsService.createPromiseDeferred();

            BusinessProcess_BPDefinitionAPIService.UpdateBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('BP Definition', response, 'Title')) {
                    if ($scope.onBPDefenitionUpdated != undefined)
                        $scope.onBPDefenitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();

                    updateBPDefintionPromiseDeferred.resolve();
                }
                else {
                    validationMessages = response.ValidationMessages;
                    updateBPDefintionPromiseDeferred.reject();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                updateBPDefintionPromiseDeferred.reject();
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            return updateBPDefintionPromiseDeferred.promise;
        }
        function getContext() {
            return {
                getDataRecordTypeId: function () {
                    return undefined;
                },
                getRecordTypeFields: function () {
                    var data = [];
                    if (vrWorkflowFields != undefined && vrWorkflowFields.length > 0) {
                        for (var i = 0; i < vrWorkflowFields.length; i++) {
                            data.push(vrWorkflowFields[i]);
                        }
                    }
                    return data;
                },
                getFields: function () {
                    var dataFields = [];
                    if (vrWorkflowFields != undefined && vrWorkflowFields.length > 0) {
                        for (var i = 0; i < vrWorkflowFields.length; i++) {
                            var field = vrWorkflowFields[i];
                            dataFields.push({
                                FieldName: field.Name,
                                FieldTitle: field.Name,
                                Type: field.Type
                            });
                        }
                    }
                    return dataFields;
                },
                getActionInfos: function () {
                    var data = [];
                    return data;
                }
            };
        }

        function buildBPEntityObjFromScope() {
            var obj;

            if (isEditMode) {
                obj = businessProcessDefinitionEntity;
            } else {
                obj = {
                    Configuration: {}
                };
            }

            obj.Title = $scope.scopeModel.title;
            obj.VRWorkflowId = $scope.scopeModel.loadVRWorklowSelector ? vrWorkflowSelectorAPI.getSelectedIds() : null;
            obj.Configuration.ProcessTitle = $scope.scopeModel.loadVRWorklowSelector ? $scope.scopeModel.processTitle : null;
            obj.Configuration.MaxConcurrentWorkflows = $scope.scopeModel.MaxConcurrentWorkflows;
            obj.Configuration.NotVisibleInManagementScreen = $scope.scopeModel.NotVisibleInManagementScreen;
            obj.Configuration.BPInstanceInsertHandler = bpInstanceInsertHandlerSettingsAPI.getData();
            obj.Configuration.ManualExecEditor = "bp-vr-workflow-manualexeceditor";
            obj.Configuration.Security = {
                View: viewPermissionAPI.getData(),
                StartNewInstance: startNewInstancePermissionAPI.getData(),
                ScheduleTask: scheduleTaskPermissionAPI.getData()
            };
            obj.Configuration.EditorSettings = editorDefinitionAPI != undefined ? editorDefinitionAPI.getData() : undefined;
            return obj;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionEditorController', BPTechnicalDefinitionEditorController);
})(appControllers);
