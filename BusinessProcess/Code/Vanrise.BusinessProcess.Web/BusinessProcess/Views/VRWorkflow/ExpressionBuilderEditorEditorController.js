(function (appControllers) {

    "use strict";

    ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var textAreaAPI;
        var expression;
        var isEditMode;
        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
        var fieldTitle;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            $scope.scopeModel = {};
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null && parameters.params != undefined) {
                var params = parameters.params;
                expression = params.expression;
                fieldTitle = params.fieldTitle;
                $scope.scopeModel.arguments = params.arguments;
                if ($scope.scopeModel.arguments != undefined && $scope.scopeModel.arguments.length > 0) {
                    $scope.scopeModel.arguments.sort(sortByName);
                }
                $scope.scopeModel.variables = params.variables;
                if ($scope.scopeModel.variables != undefined && $scope.scopeModel.variables.length > 0) {
                    $scope.scopeModel.variables.sort(sortByName);
                }
                $scope.scopeModel.fieldType = params.fieldType;
            }
            if ($scope.scopeModel.fieldType) {
                $scope.scopeModel.runTimeEditor = $scope.scopeModel.fieldType.RuntimeEditor;
                isEditMode = $scope.scopeModel.expressionValue != undefined;

            }
        }

        function defineScope() {

            $scope.scopeModel.onTextAreaReady = function (api) {
                textAreaAPI = api;
            };

            $scope.scopeModel.onFieldDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveValue = function () {
                saveValue();
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.insertText = function (item) {
                if (textAreaAPI != undefined) {
                    textAreaAPI.appendAtCursorPosition(item.Name);
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.filterValueChanged = function (value) {

                if (value == undefined || value.length == 0) {
                    setHideItemsFalse($scope.scopeModel.variables);
                    setHideItemsFalse($scope.scopeModel.arguments);
                    return;
                }

                var filter = value.toLowerCase();

                for (var i = 0; i < $scope.scopeModel.variables.length; i++) {
                    var variable = $scope.scopeModel.variables[i].Name.toLowerCase();
                    if (variable.indexOf(filter) == -1)
                        $scope.scopeModel.variables[i].hideItem = true;
                    else
                        $scope.scopeModel.variables[i].hideItem = false;
                }

                for (var j = 0; j < $scope.scopeModel.arguments.length; j++) {
                    var argument = $scope.scopeModel.arguments[j].Name.toLowerCase();
                    if (argument.indexOf(filter) == -1)
                        $scope.scopeModel.arguments[j].hideItem = true;
                    else
                        $scope.scopeModel.arguments[j].hideItem = false;
                }
            };

        }
        function loadEditorRuntimeDirective() {
            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
            runtimeEditorReadyDeferred.promise.then(function () {
                var payload = {
                    fieldType: $scope.scopeModel.fieldType,
                    fieldValue: expression != undefined ? expression.Value : undefined,
                    fieldTitle:fieldTitle
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, payload, runtimeEditorLoadDeferred);
            });

            return runtimeEditorLoadDeferred.promise;
        }
        function load() {

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                });
            }
            else
                loadAllControls();
        }
        function loadAllControls() {

            $scope.title = UtilsService.buildTitleForUpdateEditor('Expression Builder');
            $scope.scopeModel.expressionValue = expression != undefined ? expression.CodeExpression : undefined;
            var promises = [];
            if ($scope.scopeModel.fieldType != undefined)
                promises.push(loadEditorRuntimeDirective());
            return UtilsService.waitPromiseNode({ promises: promises }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function saveValue() {
            if ($scope.onSetValue != undefined) {
                if ($scope.scopeModel.expressionValue != undefined && $scope.scopeModel.expressionValue != '') {
                    $scope.onSetValue({
                        $type: "Vanrise.BusinessProcess.Entities.VRWorkflowCodeExpression, Vanrise.BusinessProcess.Entities",
                        CodeExpression: $scope.scopeModel.expressionValue
                    });
                    return;
                }

                if (runtimeEditorAPI != undefined) {
                    var fieldValue = runtimeEditorAPI.getData();
                    if (fieldValue != undefined) {
                        $scope.onSetValue({
                            $type: "Vanrise.BusinessProcess.Entities.VRWorkflowFieldTypeExpression, Vanrise.BusinessProcess.Entities",
                            Value: fieldValue,
                            FieldType: $scope.scopeModel.fieldType
                        });
                        return;
                    }
                }
                $scope.onSetValue();
            }
        }

        function setHideItemsFalse(objectsList) {

            if (objectsList == undefined || objectsList.length == 0)
                return;

            for (var i = 0; i < objectsList.length; i++)
                objectsList[i].hideItem = false;
        }

        function sortByName(a, b) {
            var nameA = a.Name.toLowerCase();
            var nameB = b.Name.toLowerCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        }
    }

    appControllers.controller('BusinessProcess_VRWorkflow_ExpressionBuilderEditorController', ExpressionBuilderEditorController);
})(appControllers);