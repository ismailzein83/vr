//(function (appControllers) {

//	"use strict";

//	ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

//	function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

//        var textAreaAPI;
//        var isEditMode;
//        var fieldValue;
//        var runtimeEditorAPI;
//        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

//		loadParameters();
//		defineScope();
//		load();

//		function loadParameters() {
//			$scope.scopeModel = {};
//			var parameters = VRNavigationService.getParameters($scope);
//			if (parameters != undefined && parameters != null) {
//				$scope.scopeModel.expressionValue = parameters.ExpressionValue;
//				$scope.scopeModel.arguments = parameters.Arguments;
//                $scope.scopeModel.variables = parameters.Variables;
//                $scope.scopeModel.fieldType = parameters.FieldType;
//                fieldValue = parameters.fieldValue;
//            }
//            if ($scope.scopeModel.fieldType) {
//                $scope.scopeModel.runTimeEditor = $scope.scopeModel.fieldType.RuntimeEditor;
//                isEditMode = $scope.scopeModel.fieldType != undefined;

//            }
//		}

//		function defineScope() {

//			$scope.scopeModel.onTextAreaReady = function (api) {
//				textAreaAPI = api;
//            };

//            $scope.scopeModel.onFieldDirectiveReady = function (api) {
//                runtimeEditorAPI = api;
//                runtimeEditorReadyDeferred.resolve();
//            };

//			$scope.scopeModel.saveValue = function () {
//                return setValue();
//			};

//			$scope.scopeModel.insertText = function (item) {
//				if (textAreaAPI != undefined) {
//					textAreaAPI.appendAtCursorPosition(item.Name);
//				}
//			};

//			$scope.scopeModel.close = function () {
//				$scope.modalContext.closeModal();
//			};
//		}
//        function loadEditorRuntimeDirective() {
//            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
//            runtimeEditorReadyDeferred.promise.then(function () {

//                var payload = {
//                    fieldType: $scope.scopeModel.fieldType,
//                    fieldValue: fieldValue,
//                };

//                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, payload, runtimeEditorLoadDeferred);
//            });

//            return runtimeEditorLoadDeferred.promise;
//        }
//        function load() {


//            $scope.scopeModel.isLoading = true;

//            if (isEditMode) {
//                loadAllControls().finally(function () {
//                });
//            }
//            else
//                loadAllControls();
//        }
//        function loadAllControls() {

//            $scope.title = UtilsService.buildTitleForUpdateEditor('Expression Builder');

//            var promises = [];
//            if ($scope.scopeModel.fieldType != undefined)
//                promises.push(loadEditorRuntimeDirective());
//            return UtilsService.waitPromiseNode({ promises: promises }).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }

//        function setValue() {
//            if ($scope.onSetExpressionBuilder != undefined && $scope.scopeModel.expressionValue != undefined && $scope.scopeModel.expressionValue!='')
//                $scope.onSetExpressionBuilder($scope.scopeModel.expressionValue);
//            if ($scope.onSetFieldValue != undefined && runtimeEditorAPI != undefined && runtimeEditorAPI.getData() != undefined) 
//                $scope.onSetFieldValue(runtimeEditorAPI.getData());
//			$scope.modalContext.closeModal();
//		}
//	}

//	appControllers.controller('BusinessProcess_VRWorkflow_FieldTypeExpressionBuilderController', ExpressionBuilderEditorController);
//})(appControllers);