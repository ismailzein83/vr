'use strict';

app.directive('vrWorkflowActivityExpressionbuilder', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_CustomCodeTaskService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_CustomCodeTaskService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '=',
				value: '=',
				label: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowAssign(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityExpressionBuilderTemplate.html'
		};

		function workflowAssign(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
			function initializeController() {
				if (ctrl.label == undefined)
					ctrl.label = "Value";
				$scope.scopeModel = {};
				$scope.scopeModel.expressionBuilderValue = ctrl.value;
				$scope.scopeModel.openExpressionBuilder = function () {
					var args = [{ Name: "arg1" }, { Name: "arg2" }, { Name: "arg3" }, { Name: "ardasdg24" }, { Name: "argfgsadhfgsdhbsdkjf16" }, { Name: "arg27" }, { Name: "arg8" }, { Name: "arg9" }, { Name: "arg19" }, { Name: "arg25" }, { Name: "arg2" }, { Name: "arg3" }, { Name: "arg24" }, { Name: "arg16" }, { Name: "arg27" }, { Name: "arg8" }, { Name: "arg9" }, { Name: "arg19" }, { Name: "arg25" }];
					var vars = [{ Name: "var1" }, { Name: "var2" }, { Name: "arg1" }, { Name: "arg2" }, { Name: "arg3" }, { Name: "ardasdg24" }, { Name: "argfgsadhfgsdhbsdkjf16" }, { Name: "arg27" }, { Name: "arg8" }, { Name: "arg9" }, { Name: "arg19" }, { Name: "arg25" }, { Name: "arg2" }, { Name: "arg3" }, { Name: "arg24" }, { Name: "arg16" }, { Name: "arg27" }, { Name: "arg8" }, { Name: "123456789012345" }, { Name: "12345678901234567" }, { Name: "1234567890123456" }];
					var onSetExpressionBuilder = function (expressionBuilderValue) {
						ctrl.value = expressionBuilderValue;
					};
					///BusinessProcess_VRWorkflowService
					BusinessProcess_VRWorkflowService.openExpressionBuilder(onSetExpressionBuilder, args, vars, ctrl.value);
				};
				defineAPI();
			}
			function defineAPI() {
				var api = {};

				/*api.load = function (payload) {
					if (payload != undefined && payload.data != undefined) {
						$scope.scopeModel.expressionBuilderValue = payload.data.ExpressionBuilderValue;
					}
				};

				api.getData = function () {
					return $scope.scopeModel.expressionBuilderValue;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);*/
			}
		}
		return directiveDefinitionObject;
	}]);