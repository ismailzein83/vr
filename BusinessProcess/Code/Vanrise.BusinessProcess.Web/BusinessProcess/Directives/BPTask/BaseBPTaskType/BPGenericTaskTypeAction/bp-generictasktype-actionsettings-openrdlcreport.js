"use strict";

app.directive("businessprocessGenerictasktypeActionsettingsOpenrdlcreport", ["UtilsService", "VRUIUtilsService",
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var generictasktypeActionExecute = new GenerictasktypeActionExecute($scope, ctrl, $attrs);
				generictasktypeActionExecute.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/BPTask/BaseBPTaskType/BPGenericTaskTypeAction/Templates/OpenRDLCReportBPGenericTaskTypeActionTemplate.html'
		};

		function GenerictasktypeActionExecute($scope, ctrl, $attrs) {
			this.initializeController = initializeController;
			var context;
			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};


				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
				};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.OpenRDLCReportBPGenericTaskTypeAction, Vanrise.BusinessProcess.MainExtensions",
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);