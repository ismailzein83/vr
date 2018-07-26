(function (appControllers) {

	"use strict";

	ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

	function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

		var textAreaAPI;
		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			$scope.scopeModel = {};
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined && parameters != null) {
				$scope.scopeModel.expressionValue = parameters.ExpressionValue;
				$scope.scopeModel.arguments = parameters.Arguments;
				$scope.scopeModel.variables = parameters.Variables;
			}
		}

		function defineScope() {

			$scope.scopeModel.onTextAreaReady = function (api) {
				textAreaAPI = api;
			};

			$scope.scopeModel.saveExpressionValue = function () {
				return setExpressionValue();
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

		function load() {
			$scope.title = UtilsService.buildTitleForUpdateEditor('Expression Builder');
		}

		function setExpressionValue() {
			if ($scope.onSetExpressionBuilder != undefined)
				$scope.onSetExpressionBuilder($scope.scopeModel.expressionValue);
			$scope.modalContext.closeModal();
		}
	}

	appControllers.controller('BusinessProcess_VRWorkflow_ExpressionBuilderController', ExpressionBuilderEditorController);
})(appControllers);