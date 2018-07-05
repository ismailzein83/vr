'use strict';

app.directive('vrWorkflowDesignerDirective', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_CustomCodeTaskService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_CustomCodeTaskService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowDesigner(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowDesignerDirectiveTemplate.html'
		};

		function workflowDesigner(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.data != undefined) {
						$scope.scopeModel.expressionBuilderValue = payload.data.ExpressionBuilderValue;
					}
				};

				api.getData = function () {
					return $scope.scopeModel.expressionBuilderValue;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);