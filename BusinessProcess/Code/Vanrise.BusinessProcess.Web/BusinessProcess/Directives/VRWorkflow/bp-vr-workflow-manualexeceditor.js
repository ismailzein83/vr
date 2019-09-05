"use strict";

app.directive("bpVrWorkflowManualexeceditor", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_BPDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_BPDefinitionAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualExecEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowManualExecEditorTemplate.html"
        };

        function ManualExecEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var runtimeEditorAPI;
            var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var bpDefinitionObj;
            var vrWorkflowFields;

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
                    var bpDefinitionId = payload.bpDefinitionId;

                    var getBPDefinitionPromise = getBPDefinition();
                    var initialPromise = [getBPDefinitionPromise];

                    var rootPromiseNode = {
                        promises: initialPromise,
                        getChildNode: function () {
                            if (bpDefinitionObj && bpDefinitionObj.Configuration && bpDefinitionObj.Configuration.ManualEditorSettings && bpDefinitionObj.Configuration.ManualEditorSettings.Enable && bpDefinitionObj.Configuration.ManualEditorSettings.EditorSettings) {
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

                    function getBPDefinition() {
                        return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
                            bpDefinitionObj = response;
                        });
                    }

                    function getVRWorkflowInputArgumentFields() {
                        return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowInputArgumentFields(bpDefinitionObj.VRWorkflowId).then(function (response) {
                            vrWorkflowFields = response;
                        });
                    }

                    function loadRuntimeEditor() {
                        var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        runtimeEditorReadyPromiseDeferred.promise.then(function () {
                            var runtimeEditorPayload = {
                                definitionSettings: bpDefinitionObj.Configuration.ManualEditorSettings.EditorSettings,
                                runtimeEditor: bpDefinitionObj.Configuration.ManualEditorSettings.EditorSettings.RuntimeEditor,
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
                    var result = {};
                    if ($scope.scopeModel.hasRuntimeEditor) {
                        var data = {};
                        runtimeEditorAPI.setData(data);
                        result.InputArguments = data;
                    }
                    return result;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
