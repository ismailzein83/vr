(function (appControllers) {

	"use strict";

	ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

	function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var textAreaAPI;
        var expression;
        var isEditMode;
        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			$scope.scopeModel = {};
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null && parameters.params != undefined) {
                var params = parameters.params;
                expression = params.expression;
                $scope.scopeModel.arguments = params.arguments;
                $scope.scopeModel.variables = params.variables;
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
		}
        function loadEditorRuntimeDirective() {
            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
            runtimeEditorReadyDeferred.promise.then(function () {
                var payload = {
                    fieldType: $scope.scopeModel.fieldType,
                    fieldValue: expression != undefined ? expression.Value : undefined,
                    fieldTitle:"Value"
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
	}

    appControllers.controller('BusinessProcess_VRWorkflow_ExpressionBuilderEditorController', ExpressionBuilderEditorController);
})(appControllers);