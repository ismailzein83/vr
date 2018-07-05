'use strict';

app.directive('vrWorkflowCustomcode', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowCustomCode(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowCustomCodeTemplate.html'
		};

		function workflowCustomCode(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.data != undefined) {
						$scope.scopeModel.customCode = payload.data.CustomCode;
					}
				};

				api.getData = function () {
					return {
						CustomCode: $scope.scopeModel.customCode,
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);