'use strict';

app.directive('vrWorkflowSequence', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '=',
				normalColNum: '@',
				dragdropsetting: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				ctrl.itemsSortable = { handle: '.handeldrag', animation: 100 };
				ctrl.itemsSortable.sort = true;
				if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
					ctrl.itemsSortable.group = {
						name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
						pull: true,
						put: ctrl.dragdropsetting.canReceive
					};

					ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
						var obj = evt.model;
						if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
							obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source);
						evt.models[evt.newIndex] = obj;
					};
				}

				var ctor = new workflowSequence(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowSequenceTemplate.html'
		};

		function workflowSequence(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
				$scope.scopeModel.onRemove = function (item) {
					var index = $scope.scopeModel.datasource.indexOf(item);
					$scope.scopeModel.datasource.splice(index, 1);
				};
				defineAPI();
			}
			function extendDataItem(dataItem) {
				dataItem.onDirectiveReady = function (api) {
					dataItem.directiveAPI = api;
					var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
					var directivePayload = {
						data: dataItem.data
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, directivePayload, setLoader);
				};
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.data != undefined) {
						for (var i = 0; i < payload.data.length; i++) {
							extendDataItem(payload.data[i]);
						}
						$scope.scopeModel.datasource = payload.data;
					}
				};

				api.getData = function () {
					var result = [];
					for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
						var item = $scope.scopeModel.datasource[i];
						var dataItem = {
							id: item.id,
							configId: item.configId,
							editor: item.editor,
							name: item.name,
							data: item.directiveAPI.getData()
						};
						result.push(dataItem);
					}
					return result;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);