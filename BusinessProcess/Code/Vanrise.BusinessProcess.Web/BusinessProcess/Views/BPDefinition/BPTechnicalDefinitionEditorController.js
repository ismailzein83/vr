(function (appControllers) {

    "use strict";

    BPTechnicalDefinitionEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService','VRCommon_FieldTypesService'];

    function BPTechnicalDefinitionEditorController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, VRCommon_FieldTypesService) {

        var isEditMode;
        var vrWorkflowArguments;
        var validationMessages;

        var businessProcessDefinitionId;
        var businessProcessDefinitionEntity;

        var modalWidthSelectorAPI;
        var modalWidthSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

        var manualEditorDefinitionAPI;
        var manualEditorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var scheduleEditorDefinitionAPI;
        var scheduleEditorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        var processTitleExpressionBuilderDirectiveAPI;
        var processTitleExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var vrWorkflowFields;
        var textFieldType = VRCommon_FieldTypesService.getTextFieldType();
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

            $scope.scopeModel.onModalWidthSelectorReady = function (api) {
                modalWidthSelectorAPI = api;
                modalWidthSelectorReadyPromiseDeferred.resolve();
            };

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

            $scope.scopeModel.onManualEditorDefinitionDirectiveReady = function (api) {
                manualEditorDefinitionAPI = api;
                manualEditorDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onScheduledEditorDefinitionDirectiveReady = function (api) {
                scheduleEditorDefinitionAPI = api;
                scheduleEditorDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };

            $scope.scopeModel.onProcessTitleExpressionBuilderDirectiveReady = function (api) {
                processTitleExpressionBuilderDirectiveAPI = api;
                processTitleExpressionBuilderPromiseReadyDeffered.resolve();
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
                            if (processTitleExpressionBuilderDirectiveAPI != undefined) {
                                processTitleExpressionBuilderDirectiveAPI.load({
                                    context: getContext(),
                                    value: undefined,
                                    fieldEntity: {
                                        fieldType: textFieldType,
                                        fieldTitle: "Process Title"
                                    }
                                });
                            }
                            $scope.scopeModel.isLoadingArguments = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            $scope.scopeModel.isLoadingArguments = false;
                        });
                        getVRWorkflowInputArgumentFields(selectedVRWorkflow.VRWorkflowId).then(function () {
                            modalWidthSelectorReadyPromiseDeferred.promise.then(function () {
                                var setLoader = function (value) { $scope.scopeModel.isModalWidthSelectorLoading = value };
                                var modalWidthSelectorPayload = {
                                    selectedIds: businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined ? businessProcessDefinitionEntity.Configuration.EditorSize : undefined
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, modalWidthSelectorAPI, modalWidthSelectorPayload, setLoader);
                            });
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

            $scope.scopeModel.onManualEditorSwitchValueChanged = function () {
                if ($scope.scopeModel.manualEditorDefinition) {
                    manualEditorDefinitionReadyPromiseDeferred.promise.then(function () {
                        var setEditorLoader = function (value) { $scope.scopeModel.isManualEditorLoading = value; };
                        var manualEditorPayload = {
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, manualEditorDefinitionAPI, manualEditorPayload, setEditorLoader);
                    });
                }
                else {
                    $scope.scopeModel.sameAsManualEditorDefinition = false;
                    if ($scope.scopeModel.scheduleEditorDefinition) {
                        loadScheduleEditorSelector();
                    }
                }
            };

            $scope.scopeModel.onScheduleEditorSwitchValueChanged = function () {
                if ($scope.scopeModel.scheduleEditorDefinition) {
                    loadScheduleEditorSelector();
                }
                else {
                    $scope.scopeModel.sameAsManualEditorDefinition = false;
                }
            };

            $scope.scopeModel.onSameAsManualSwitchValueChanged = function () {
                if (!$scope.scopeModel.sameAsManualEditorDefinition) {
                    if ($scope.scopeModel.scheduleEditorDefinition)
                        loadScheduleEditorSelector();
                }
            };
        }

        function loadScheduleEditorSelector() {
            scheduleEditorDefinitionReadyPromiseDeferred.promise.then(function () {
                var setEditorLoader = function (value) { $scope.scopeModel.isScheduleEditorLoading = value; };
                var scheduleEditorPayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, scheduleEditorDefinitionAPI, scheduleEditorPayload, setEditorLoader);
            });
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

        function loadManualEditorDefinitionDirective() {
            var loadManualEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            manualEditorDefinitionReadyPromiseDeferred.promise.then(function () {
                var settingsEditor;
                if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined) {
                    if (businessProcessDefinitionEntity.Configuration.ManualEditorSettings)
                        settingsEditor = businessProcessDefinitionEntity.Configuration.ManualEditorSettings;
                }
                var editorPayload = {
                    settings: settingsEditor ? settingsEditor.EditorSettings : undefined,
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoad(manualEditorDefinitionAPI, editorPayload, loadManualEditorDefinitionDirectivePromiseDeferred);
            });
            return loadManualEditorDefinitionDirectivePromiseDeferred.promise;
        }

        function loadScheduleEditorDefinitionDirective() {
            var loadScheduleEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            scheduleEditorDefinitionReadyPromiseDeferred.promise.then(function () {
                var settingsEditor;
                if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined) {
                    if (businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings)
                        settingsEditor = businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings;
                }
                var editorPayload = {
                    settings: settingsEditor != undefined ? settingsEditor.EditorSettings : undefined,
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoad(scheduleEditorDefinitionAPI, editorPayload, loadScheduleEditorDefinitionDirectivePromiseDeferred);
            });
            return loadScheduleEditorDefinitionDirectivePromiseDeferred.promise;
        }

        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (businessProcessDefinitionEntity != undefined) {
                    payloadDirective = {
                        selectedIds: businessProcessDefinitionEntity.DevProjectId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, payloadDirective, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }

        function loadProcessTitleExpressionBuilder() {
            var processTitleExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
            processTitleExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                var processTitlePayload = {
                    context: getContext(),
                    value: businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined ? businessProcessDefinitionEntity.Configuration.ProcessTitle : undefined,
                    fieldEntity: {
                        fieldType: textFieldType,
                        fieldTitle:"Process Title"
                    }
                };
                VRUIUtilsService.callDirectiveLoad(processTitleExpressionBuilderDirectiveAPI, processTitlePayload, processTitleExpressionBuilderPromiseLoadDeffered);
            });
            return processTitleExpressionBuilderPromiseLoadDeffered.promise;
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
                $scope.scopeModel.warningMessage = businessProcessDefinitionEntity.Configuration.WarningMessage;
                $scope.scopeModel.manualEditorDefinition = businessProcessDefinitionEntity.Configuration.ManualEditorSettings && businessProcessDefinitionEntity.Configuration.ManualEditorSettings.Enable || false;
                $scope.scopeModel.scheduleEditorDefinition = businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings && businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings.Enable || false;
                $scope.scopeModel.sameAsManualEditorDefinition = businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings && businessProcessDefinitionEntity.Configuration.ScheduleEditorSettings.SameAsManualEditor || false;
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
            function loadModalWidthSelector() {
                if (!isEditMode || businessProcessDefinitionEntity.VRWorkflowId == undefined)
                    return;

                var loadModalWidthSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                modalWidthSelectorReadyPromiseDeferred.promise.then(function () {
                    vrWorkflowSelectorSelectionChangedDeferred = undefined;
                    var selectorPayload = {
                        selectedIds: businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined ? businessProcessDefinitionEntity.Configuration.EditorSize : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(modalWidthSelectorAPI, selectorPayload, loadModalWidthSelectorPromiseDeferred);
                });

                return loadModalWidthSelectorPromiseDeferred.promise;
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
            var operations = [setTitle, loadStaticData, loadVRWorkflowSelector, loadModalWidthSelector, GetVRWorkflowArguments, loadViewRequiredPermission, loadStartNewInstanceRequiredPermission,
                loadScheduleTaskRequiredPermission, loadBPInstanceInsertHandlerSettings, loadDevProjectSelector];
            if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.VRWorkflowId != undefined) {
                operations.push(getVRWorkflowInputArgumentFields);

                var config = businessProcessDefinitionEntity.Configuration;
                if (config != undefined) {
                    if (config.ManualEditorSettings != undefined) {
                        if (config.ManualEditorSettings.Enable)
                            operations.push(loadManualEditorDefinitionDirective);
                    }
                    if (config.ScheduleEditorSettings != undefined) {
                        if (config.ScheduleEditorSettings.Enable && !config.ScheduleEditorSettings.SameAsManualEditor)
                            operations.push(loadScheduleEditorDefinitionDirective);
                    }
                }

            }
            if ($scope.scopeModel.loadVRWorklowSelector) {
                operations.push(loadProcessTitleExpressionBuilder);
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
                getFieldType: function (fieldName) {
                    for (var i = 0; i < vrWorkflowFields.length; i++) {
                        var field = vrWorkflowFields[i];
                        if (field.Name == fieldName)
                            return field.Type;
                    }
                },
                getActionInfos: function () {
                    var data = [];
                    return data;
                },
                getWorkflowArguments: function () {
                    return vrWorkflowArguments;
                },
                getParentVariables: function () {
                    return null;
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
            obj.DevProjectId = devProjectDirectiveApi.getSelectedIds();
            obj.Configuration.ProcessTitle = $scope.scopeModel.loadVRWorklowSelector && processTitleExpressionBuilderDirectiveAPI != undefined ? processTitleExpressionBuilderDirectiveAPI.getData() : null;
            obj.Configuration.EditorSize = modalWidthSelectorAPI != undefined ? modalWidthSelectorAPI.getSelectedIds() : null;
            obj.Configuration.MaxConcurrentWorkflows = $scope.scopeModel.MaxConcurrentWorkflows;
            obj.Configuration.NotVisibleInManagementScreen = $scope.scopeModel.NotVisibleInManagementScreen;
            obj.Configuration.BPInstanceInsertHandler = bpInstanceInsertHandlerSettingsAPI.getData();

            if (obj.VRWorkflowId != undefined)
                obj.Configuration.ManualExecEditor = $scope.scopeModel.manualEditorDefinition ? undefined : "bp-vr-workflow-manualexeceditor";

            obj.Configuration.WarningMessage = $scope.scopeModel.warningMessage;
            obj.Configuration.Security = {
                View: viewPermissionAPI.getData(),
                StartNewInstance: startNewInstancePermissionAPI.getData(),
                ScheduleTask: scheduleTaskPermissionAPI.getData()
            };

            var manualEditorSettings = $scope.scopeModel.manualEditorDefinition ? manualEditorDefinitionAPI.getData() : undefined;

            obj.Configuration.ManualEditorSettings = {
                Enable: $scope.scopeModel.manualEditorDefinition,
                EditorSettings: manualEditorSettings
            };

            obj.Configuration.ScheduleEditorSettings = {
                Enable: $scope.scopeModel.scheduleEditorDefinition,
                SameAsManualEditor: $scope.scopeModel.sameAsManualEditorDefinition
            };

            if ($scope.scopeModel.sameAsManualEditorDefinition)
                obj.Configuration.ScheduleEditorSettings.EditorSettings = manualEditorSettings;
            else {
                obj.Configuration.ScheduleEditorSettings.EditorSettings = $scope.scopeModel.scheduleEditorDefinition ? scheduleEditorDefinitionAPI.getData() : undefined;
            }

            return obj;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionEditorController', BPTechnicalDefinitionEditorController);
})(appControllers);

