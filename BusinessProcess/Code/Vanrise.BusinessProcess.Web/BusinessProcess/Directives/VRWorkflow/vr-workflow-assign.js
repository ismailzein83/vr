'use strict';

app.directive('vrWorkflowAssign', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '='
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
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowAssignTemplate.html'
		};

		function workflowAssign(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.data != undefined) {
						$scope.scopeModel.to = payload.data.To;
						$scope.scopeModel.value = payload.data.Value;
					}
				};

				api.getData = function () {
					console.log("r  "+$scope.x);
					return {
						To: $scope.scopeModel.to,
						Value: $scope.scopeModel.value
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);