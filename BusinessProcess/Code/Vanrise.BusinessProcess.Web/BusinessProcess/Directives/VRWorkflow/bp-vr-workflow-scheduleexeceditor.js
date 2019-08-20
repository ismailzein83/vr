"use strict";

app.directive("bpVrWorkflowScheduleexeceditor", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_VRWorkflowAPIService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_VRWorkflowAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ScheduleExecEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowScheduleExecEditorTemplate.html"
        };

        function ScheduleExecEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var runtimeEditorAPI;
            var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var bpDefinitionId
            var vrWorkflowFields;
            var bpDefinitionObj;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    runtimeEditorAPI = api;
                    runtimeEditorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    bpDefinitionId = payload.bpDefinitionId;
                    var data = payload.data;

                    var inputArguments;
                    if (data != undefined && data.InputArguments != undefined) {
                        inputArguments = data.InputArguments;
                    }

                    var getBPDefintionPromiseDeferred = UtilsService.createPromiseDeferred();

                    BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
                        bpDefinitionObj = response;
                        getBPDefintionPromiseDeferred.resolve();
                    });

                    var rootPromiseNode = {
                        promises: [getBPDefintionPromiseDeferred.promise],
                        getChildNode: function () {
                            if (bpDefinitionObj && bpDefinitionObj.Configuration && bpDefinitionObj.Configuration.ScheduleEditorSettings && bpDefinitionObj.Configuration.ScheduleEditorSettings.Enable) {
                                $scope.scopeModel.hasRuntimeEditor = true;
                                var getVRWorkflowInputArgumentFieldsPromise = getVRWorkflowInputArgumentFields();

                                return {
                                    promises: [getVRWorkflowInputArgumentFieldsPromise],
                                    getChildNode: function () {
                                        var loadRuntimeEditorPromise = loadRuntimeEditor();
                                        return {
                                            promises: [loadRuntimeEditorPromise]
                                        };
                                    }
                                };
                            }
                            else {
                                return {
                                    promises: []
                                };
                            }
                        }
                    };

                    function getVRWorkflowInputArgumentFields() {
                        return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowInputArgumentFields(bpDefinitionObj.VRWorkflowId).then(function (response) {
                            vrWorkflowFields = response;
                        });
                    }

                    function loadRuntimeEditor() {
                        var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        runtimeEditorReadyPromiseDeferred.promise.then(function () {
                            var runtimeEditorPayload = {
                                definitionSettings: bpDefinitionObj.Configuration.ScheduleEditorSettings.EditorSettings,
                                runtimeEditor: bpDefinitionObj.Configuration.ScheduleEditorSettings.EditorSettings.RuntimeEditor,
                                selectedValues: inputArguments,
                                context: buildContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadPromiseDeferred);
                        });

                        return runtimeEditorLoadPromiseDeferred.promise;
                    }

                    function buildContext() {
                        return {
                            getFields: function () {
                                if (vrWorkflowFields == undefined || vrWorkflowFields.length == 0)
                                    return undefined;

                                var fields = [];
                                for (var i = 0; i < vrWorkflowFields.length; i++) {
                                    var field = UtilsService.cloneObject(vrWorkflowFields[i], false);
                                    fields.push({
                                        Name: field.Name,
                                        Type: field.Type
                                    });
                                }

                                return fields;
                            }
                        };
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    if ($scope.scopeModel.hasRuntimeEditor) {
                        var inputArguments = {};
                        runtimeEditorAPI.setData(inputArguments);

                        var data = {
                            $type: "Vanrise.BusinessProcess.Entities.VRWorkflowDictInputArgument, Vanrise.BusinessProcess.Entities",
                            InputArguments: inputArguments,
                            BPDefinitionId: bpDefinitionId
                        };

                        return data;
                    }
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
