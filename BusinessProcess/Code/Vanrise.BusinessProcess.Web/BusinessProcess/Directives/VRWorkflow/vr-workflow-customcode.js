'use strict';

app.directive('businessprocessVrWorkflowCustomcode', ['UtilsService', 'VRUIUtilsService',
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
					if (payload != undefined) {
						if (payload.Settings != undefined && payload.Settings.Code != null) {
							$scope.scopeModel.customCode = payload.Settings.Code;
						}
						if (payload.Context != null) 
							$scope.scopeModel.context = payload.Context;
					}
				};

				api.getData = function () {
					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCustomLogicActivity, Vanrise.BusinessProcess.MainExtensions",
						Code: $scope.scopeModel.customCode
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);