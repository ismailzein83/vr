(function (appControllers) {

    "use strict";

    SubprocessEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService'];

    function SubprocessEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {


        var vrWorkflowSelectorAPI;
        var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var vrWorkflowSelectorSelectionChangedDeferred;

        var inArgumentsGridAPI;
        var inArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var outArgumentsGridAPI;
        var outArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var selectedVRWrkflow;

        var loadSelectedVRWorkflowPromise;

        var selectedVRWorkflowId;
        var inArguments;
        var outArguments;
        var workflowId;
        var isNew;
        var context;
        var displayName;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                isNew = parameters.isNew;
                if (parameters.obj != undefined) {
                    selectedVRWorkflowId = parameters.obj.VRWorkflowId;
                    inArguments = parameters.obj.InArguments;
                    outArguments = parameters.obj.OutArguments;
                    workflowId = parameters.workflowId;
                    context = parameters.context;
                    displayName = parameters.obj.DisplayName;
                }
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.hasInArguments = false;
            $scope.scopeModel.hasOutArguments = false;
            $scope.scopeModel.inArguments = [];
            $scope.scopeModel.outArguments = [];

            $scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
                vrWorkflowSelectorAPI = api;
                vrWorkflowSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onInArgumentsGridReady = function (api) {
                inArgumentsGridAPI = api;
                inArgumentsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onOutArgumentsGridReady = function (api) {
                outArgumentsGridAPI = api;
                outArgumentsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onVRWorkflowSelectionChanged = function (selectedItem) {
                if (selectedItem == undefined)
                    return;
                else {
                    if (vrWorkflowSelectorSelectionChangedDeferred != undefined) {
                        vrWorkflowSelectorSelectionChangedDeferred.resolve();
                    }
                    else {
                        $scope.scopeModel.isLoading = true;
                        var rootPromiseNode = {
                            promises: [loadVRWorkflow(selectedItem.VRWorkflowId)],
                            getChildNode: function () {
                                var gridPromises = [];
                                if ($scope.scopeModel.hasInArguments)
                                    gridPromises.push(loadInArgumentsGrid(undefined, gridPromises));

                                if ($scope.scopeModel.hasOutArguments)
                                    gridPromises.push(loadOutArgumentsGrid(undefined, gridPromises));

                                return {
                                    promises: gridPromises
                                };
                            }
                        };
                        UtilsService.waitPromiseNode(rootPromiseNode).then(function () { $scope.scopeModel.isLoading = false; });
                    }
                }
            };

            $scope.scopeModel.close = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
                $scope.modalContext.closeModal();
            };

            $scope.modalContext.onModalHide = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
            };

            $scope.scopeModel.saveActivity = function () {
                return updateActivity();
            };

            $scope.scopeModel.displayName = displayName;
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Edit Sub Process";
            }

            function loadVRWorkflowInfo() {
                var promises = [];

                var loadVRWorkflowSelectorPromise = loadVRWorkflowSelector();
                promises.push(loadVRWorkflowSelectorPromise);

                if (selectedVRWorkflowId != undefined) {
                    vrWorkflowSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                    var rootPromiseNode = {
                        promises: [vrWorkflowSelectorSelectionChangedDeferred.promise],
                        getChildNode: function () {
                            return {
                                promises: [loadVRWorkflow(selectedVRWorkflowId)],
                                getChildNode: function () {
                                    var gridPromises = [];
                                    if ($scope.scopeModel.hasInArguments)
                                        gridPromises.push(loadInArgumentsGrid(inArguments, gridPromises));

                                    if ($scope.scopeModel.hasOutArguments)
                                        gridPromises.push(loadOutArgumentsGrid(outArguments, gridPromises));

                                    return {
                                        promises: gridPromises
                                    };
                                }
                            };
                        }
                    };
                    promises.push(UtilsService.waitPromiseNode(rootPromiseNode));
                }
                return UtilsService.waitMultiplePromises(promises);
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadVRWorkflowInfo]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                vrWorkflowSelectorSelectionChangedDeferred = undefined;
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadVRWorkflowSelector() {
            var vrWorkflowSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            vrWorkflowSelectorReadyDeferred.promise.then(function () {
                var vrWorkflowSelectorPayload = {};

                if (selectedVRWorkflowId != undefined)
                    vrWorkflowSelectorPayload.selectedIds = selectedVRWorkflowId;

                if (workflowId != undefined)
                    vrWorkflowSelectorPayload.filter = { ExcludedIds: [workflowId] };

                VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowSelectorLoadDeferred);
            });

            return vrWorkflowSelectorLoadDeferred.promise;
        }

        function updateActivity() {
            $scope.scopeModel.isLoading = true;
            var updatedObject = buildObjFromScope();
            if ($scope.onActivityUpdated != undefined) {
                $scope.onActivityUpdated(updatedObject);
            }
            $scope.scopeModel.isLoading = false;
            isNew = false;
            $scope.modalContext.closeModal();
        }

        function buildObjFromScope() {
            var inArgumentsObject;
            if ($scope.scopeModel.hasInArguments) {
                inArgumentsObject = {};
                for (var x = 0; x < $scope.scopeModel.inArguments.length; x++) {
                    var currentInArgument = $scope.scopeModel.inArguments[x];
                    if (currentInArgument.valueExpressionBuilderDirectiveAPI != undefined) {
                        var value = currentInArgument.valueExpressionBuilderDirectiveAPI.getData();
                        if (value != undefined)
                            inArgumentsObject[currentInArgument.entity.name] = value;
                    }
                }
            }

            var outArgumentsObject;
            if ($scope.scopeModel.hasOutArguments) {
                outArgumentsObject = {};
                for (var x = 0; x < $scope.scopeModel.outArguments.length; x++) {
                    var currentOutArgument = $scope.scopeModel.outArguments[x];
                    if (currentOutArgument.valueExpressionBuilderDirectiveAPI != undefined) {
                        var value = currentOutArgument.valueExpressionBuilderDirectiveAPI.getData();
                        if (value != undefined)
                            outArgumentsObject[currentOutArgument.entity.name] = value;
                    }
                }
            }

            return {
                VRWorkflowName: ($scope.scopeModel.selectedWorkflow != undefined) ? $scope.scopeModel.selectedWorkflow.Name : null,
                VRWorkflowId: vrWorkflowSelectorAPI.getSelectedIds(),
                InArguments: inArgumentsObject,
                OutArguments: outArgumentsObject,
                DisplayName: $scope.scopeModel.displayName
            };
        }

        function loadVRWorkflow(vrWorkflowId) {
            $scope.scopeModel.hasInArguments = false;
            $scope.scopeModel.hasOutArguments = false;

            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowEditorRuntime(vrWorkflowId).then(function (response) {
                selectedVRWrkflow = response.Entity;
                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
                        switch (currentArgumentDefinition.Direction) {
                            case 0: $scope.scopeModel.hasInArguments = true; break;
                            case 1: $scope.scopeModel.hasOutArguments = true; break;
                            case 2: $scope.scopeModel.hasInArguments = true; $scope.scopeModel.hasOutArguments = true; break;
                        }

                    }
                }
            });
        }

        function loadInArgumentsGrid(selectedArguments,gridPromises) {
            $scope.scopeModel.inArguments = [];
            var inArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
            inArgumentsGridReadyDeferred.promise.then(function () {
                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
                        if (currentArgumentDefinition.Direction != 1) {
                            var argumentObject = {
                                payload: currentArgumentDefinition,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            gridPromises.push(argumentObject.loadPromiseDeferred.promise);
                            prepareInArgument(argumentObject, selectedArguments);
                        }
                    }
                }
                inArgumentsGridLoadDeferred.resolve();
            });

            return inArgumentsGridLoadDeferred.promise;
        }
        function prepareInArgument(argumentObject, selectedArguments) {
            var dataItem = {
                entity: {
                    name: argumentObject.payload.Name
                }
            };
            var argValue = tryGetArgumentValue(selectedArguments, argumentObject.payload.Name);
            dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                dataItem.valueExpressionBuilderDirectiveAPI = api;
                argumentObject.readyPromiseDeferred.resolve();
            }; 
            argumentObject.readyPromiseDeferred.promise.then(function () {
                var payload = {
                    context: context,
                    value: argValue,
                    fieldEntity: {
                        fieldType: argumentObject.payload.Type != undefined ? argumentObject.payload.Type.FieldType : undefined,
                        fieldTitle: argumentObject.payload.Name
                    }
                };
                VRUIUtilsService.callDirectiveLoad(dataItem.valueExpressionBuilderDirectiveAPI, payload, argumentObject.loadPromiseDeferred);
            });
              
            $scope.scopeModel.inArguments.push(dataItem);
        }
        function tryGetArgumentValue(selectedArguments, argumentName) {
            if (selectedArguments == undefined)
                return;

            return selectedArguments[argumentName];
        }

        function loadOutArgumentsGrid(selectedArguments, gridPromises) {
            $scope.scopeModel.outArguments = [];
            var outArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
            outArgumentsGridReadyDeferred.promise.then(function () {
                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
                        if (currentArgumentDefinition.Direction != 0) {
                            var argumentObject = {
                                payload: currentArgumentDefinition,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            gridPromises.push(argumentObject.loadPromiseDeferred.promise);
                            prepareOutArgument(argumentObject, selectedArguments);
                        }
                    }
                }
                outArgumentsGridLoadDeferred.resolve();
            });

            return outArgumentsGridLoadDeferred.promise;
        }
        function prepareOutArgument(argumentObject, selectedArguments) {
            var dataItem = {
                entity: {
                    name: argumentObject.payload.Name
                }
            };
            var argValue = tryGetArgumentValue(selectedArguments,argumentObject.payload.Name);
            dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                dataItem.valueExpressionBuilderDirectiveAPI = api;
                argumentObject.readyPromiseDeferred.resolve();
            };
            argumentObject.readyPromiseDeferred.promise.then(function () {
                var payload = {
                    context: context,
                    value: argValue
                };
                VRUIUtilsService.callDirectiveLoad(dataItem.valueExpressionBuilderDirectiveAPI, payload, argumentObject.loadPromiseDeferred);
            });
            $scope.scopeModel.outArguments.push(dataItem);
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowActivitySubprocessController', SubprocessEditorController);
})(appControllers);