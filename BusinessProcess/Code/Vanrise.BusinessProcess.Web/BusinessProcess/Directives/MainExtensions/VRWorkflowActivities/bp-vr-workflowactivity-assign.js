'use strict';

app.directive('businessprocessVrWorkflowactivityAssign', ['UtilsService', 'VRUIUtilsService',
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
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowAssignTemplate.html'
		};

		function workflowAssign(ctrl, $scope, $attrs) {
			var assignListItems = [];

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = { items: [] };
				$scope.scopeModel.isVRWorkflowActivityDisabled = false;

				$scope.scopeModel.addAssign = function () {
					var assignItem = {};
					$scope.scopeModel.items.push(assignItem);
					assignListItems.push(assignItem);
				};

				$scope.scopeModel.removeAssign = function (item) {
					$scope.scopeModel.items.splice($scope.scopeModel.items.indexOf(item), 1);
					assignListItems.splice(assignListItems.indexOf(item), 1);
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};
				api.load = function (payload) {
					var promises = [];
					if (payload != undefined) {
						if (payload.Settings != undefined) {
							$scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;

							if (payload.Settings.Items != null && payload.Settings.Items.length > 0) {
								for (var x = 0; x < payload.Settings.Items.length; x++) {
									var currentAssign = payload.Settings.Items[x];
									var itemToAdd = { to: currentAssign.To, value: currentAssign.Value };
									$scope.scopeModel.items.push(itemToAdd);
									assignListItems.push(itemToAdd);
								}
							}
							else {
								var initialAssignItem = {};
								$scope.scopeModel.items.push(initialAssignItem);
								assignListItems.push(initialAssignItem);
							}
						}
						if (payload.Context != null)
							$scope.scopeModel.context = payload.Context;
					}
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var items = [];

					for (var x = 0; x < assignListItems.length; x++) {
						var currentAssign = assignListItems[x];
						items.push({ To: currentAssign.to, Value: currentAssign.value });
					}

					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowAssignActivity, Vanrise.BusinessProcess.MainExtensions",
						Items: items
					};
				};

				api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
					$scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
				};

				api.getActivityStatus = function () {
					return $scope.scopeModel.isVRWorkflowActivityDisabled;
				};

				api.isActivityValid = function () {
					if ($scope.scopeModel.items == undefined || $scope.scopeModel.items.length == 0)
						return false;

					for (var x = 0; x < $scope.scopeModel.items.length; x++) {
						var currentAssign = $scope.scopeModel.items[x];
						if (currentAssign.to == undefined || currentAssign.value == undefined)
							return false;
					}

					return true;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);