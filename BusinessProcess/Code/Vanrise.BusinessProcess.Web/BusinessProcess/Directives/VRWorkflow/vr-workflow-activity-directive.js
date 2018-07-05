"use strict";

app.directive("vrWorkflowActivityDirective", ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				dragdropsetting: '=',
				onRemove: '=',
				onReady: "=",
				dataitem: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityDirectiveTemplate.html'
		};

		function DirectiveConstructor($scope, ctrl) {
			var directiveWraperAPI;
			var directiveWraperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
				$scope.scopeModel.editor = ctrl.dataitem.editor;
				$scope.scopeModel.onRemove = function () { ctrl.onRemove(ctrl.dataitem); };
				$scope.scopeModel.header = ctrl.dataitem.name || ctrl.dataitem.Title;
				$scope.scopeModel.onReady = ctrl.onReady;
				$scope.scopeModel.onDirectiveWraperReady = function (api) {
					directiveWraperAPI = api;
					directiveWraperReadyPromiseDeferred.resolve();
				};
				//defineAPI();
			}

			function defineAPI() {

				var api = {};
				api.load = function (payload) {
					directiveWraperReadyPromiseDeferred.promise.then(function () {
						directiveWraperAPI.load(payload);
					});
				};

				api.getData = function () {
					if (directiveWraperAPI != null)
						directiveWraperAPI.getData();
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
