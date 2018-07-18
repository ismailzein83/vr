'use strict';

app.directive('businessprocessVrWorkflowAssign', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '='
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
					if (payload != undefined) {
						if (payload.Settings != undefined && payload.Settings.Items != null && payload.Settings.Items.length > 0) {
							$scope.scopeModel.to = payload.Settings.Items[0].To;
							$scope.scopeModel.value = payload.Settings.Items[0].Value;
						}
						if (payload.Context != null) 
							$scope.scopeModel.context = payload.Context;
					}
				};

				api.getData = function () {
					var items = [{
						To: $scope.scopeModel.to,
						Value: $scope.scopeModel.value
					}];
					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowAssignActivity, Vanrise.BusinessProcess.MainExtensions",
						Items: items
						//Title: "Assign",
						//Editor: "businessprocess-vr-workflow-assign"
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);