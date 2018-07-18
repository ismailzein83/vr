(function (appControllers) {

	"use strict";

	ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

	function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

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
			$scope.scopeModel.saveExpressionValue = function () {
				return setExpressionValue();
			};

			$scope.scopeModel.insertText = function (item) {
				var textToAdd = item.Name;
				var input = document.getElementById("resid");
				if (input == undefined) { return; }
				var scrollTop = input.scrollTop;
				var cursorPosition = 0;
				var browser = ((input.selectionStart || input.selectionStart == "0") ? "ch" : (document.selection ? "IE" : false));
				var inputLength = input.value.length;
				if (browser == "IE") {
					input.focus();
					var range = document.selection.createRange();
					range.moveStart("character", -inputLength);
					cursorPosition = range.text.length;
				}
				else if (browser == "ch") { cursorPosition = input.selectionStart; }

				var pre = (input.value).substring(0, cursorPosition);
				var post = (input.value).substring(cursorPosition, inputLength);
				input.value = pre + textToAdd + post;
				cursorPosition = cursorPosition + textToAdd.length;
				if (browser == "IE") {
					input.focus();
					var range = document.selection.createRange();
					range.moveStart("character", -inputLength);
					range.moveStart("character", cursorPosition);
					range.moveEnd("character", 0);
					range.select();
				}
				else if (browser == "ch") {
					input.selectionStart = cursorPosition;
					input.selectionEnd = cursorPosition;
					input.focus();
				}
				input.scrollTop = scrollTop;
				angular.element(input).trigger('input');
			};

			$scope.scopeModel.itemClicked = function (item) {
				if ($scope.scopeModel.expressionValue == undefined)
					$scope.scopeModel.expressionValue = item.Name;
				else
					$scope.scopeModel.expressionValue += item.Name;
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