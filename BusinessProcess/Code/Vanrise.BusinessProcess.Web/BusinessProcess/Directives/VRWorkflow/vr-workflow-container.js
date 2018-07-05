"use strict";

app.directive('vrWorkflowContainer', ['UtilsService', 'VRUIUtilsService', 
	function (UtilsService, VRUIUtilsService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "=",
				dragdropsetting: '=',
				datasource: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				ctrl.itemsSortable = { handle: '.handeldrag', animation: 100 };
				ctrl.itemsSortable.sort = true;
				if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
					ctrl.itemsSortable.group = {
						name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
						pull: true,
						put: function (to) {
							return (ctrl.dragdropsetting.canReceive && to.el.children.length < 1);
						} 
					};

					ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
						if (evt.models.length >= 1)
							return;
						var obj = evt.model;
						if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
							obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source);
						evt.models[evt.newIndex] = obj;
					};
				}

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowContainerTemplate.html'
		};

		function DirectiveConstructor($scope, ctrl) {
			if (ctrl.datasource == undefined)
				ctrl.datasource = [];
			this.initializeController = initializeController;
			var context;
			var compiled = false;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.onRemove = function (item) {
					var index = ctrl.datasource.indexOf(item);
					ctrl.datasource.splice(index, 1);
				};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
				defineAPI();
			}

			function defineAPI() {

				var api = {};
				api.load = function (payload) {

				};

				api.getData = function () {

				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
