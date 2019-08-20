"use strict";

app.directive("bpVrWorkflowManualexeceditor", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {
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
                    bpDefinitionObj = payload.bpDefinitionObj;

                    var rootPromiseNode = {
                        promises: []
                    };

                    if (bpDefinitionObj && bpDefinitionObj.Configuration && bpDefinitionObj.Configuration.ManualEditorSettings && bpDefinitionObj.Configuration.ManualEditorSettings.Enable) {
                        $scope.scopeModel.hasRuntimeEditor = true;
                        var getVRWorkflowInputArgumentFieldsPromise = getVRWorkflowInputArgumentFields()
                        rootPromiseNode.promises.push(getVRWorkflowInputArgumentFieldsPromise);

                        rootPromiseNode.getChildNode = function () {
                            var loadRuntimeEditorPromise = loadRuntimeEditor();
                            return {
                                promises: [loadRuntimeEditorPromise]
                            };
                        };
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
                    if ($scope.scopeModel.hasRuntimeEditor) {
                        var data = {};
                        runtimeEditorAPI.setData(data);

                        return {
                            InputArguments: data
                        };
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
