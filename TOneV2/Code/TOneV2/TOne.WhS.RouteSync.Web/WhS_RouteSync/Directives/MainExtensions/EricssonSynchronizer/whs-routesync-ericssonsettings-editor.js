(function (app) {

	'use strict';

	EricssonSWSyncEricssonSettings.$inject = ["UtilsService", 'VRUIUtilsService'];

	function EricssonSWSyncEricssonSettings(UtilsService, VRUIUtilsService) {
		return {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new EricssonSWSyncEricssonSettingsrCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonRouteSyncSetting.html"
		};

		function EricssonSWSyncEricssonSettingsrCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;


			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.faultCodes = [];
				$scope.scopeModel.faultCodesA = [];
				$scope.scopeModel.faultCodeValue = undefined;
				$scope.scopeModel.disableAddButton = true;

				$scope.scopeModel.onFaultCodeValueChange = function (value) {

					if (value == undefined) {
						$scope.scopeModel.disableAddButton = true;
						return;
					}

					if ($scope.scopeModel.faultCodes == undefined || $scope.scopeModel.faultCodes.length == 0) {
						$scope.scopeModel.disableAddButton = false;
						return;
					}

					for (var i = 0; i < $scope.scopeModel.faultCodes.length; i++) {
						if ($scope.scopeModel.faultCodes[i].faultCode == $scope.scopeModel.faultCodeValue) {
							$scope.scopeModel.disableAddButton = true;
							return;
						}
					}
					$scope.scopeModel.disableAddButton = false;
				};

				$scope.scopeModel.addFaultCodeValue = function () {

					$scope.scopeModel.faultCodes.push({
						faultCode: $scope.scopeModel.faultCodeValue
					});
					$scope.scopeModel.faultCodeValue = undefined;
					$scope.scopeModel.disableAddButton = true;
				};

				$scope.scopeModel.validateFaultCodes = function () {
					if ($scope.scopeModel.faultCodes == undefined || $scope.scopeModel.faultCodes.length == 0)
						return 'Please, add fault codes';
					return null;
				};

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					if (payload != undefined) {
						$scope.scopeModel.numberOfRetries = payload.settings.NumberOfRetries;
						if (payload.settings.FaultCodes != undefined) {
							$scope.scopeModel.faultCodesA = payload.settings.FaultCodes;

						}
					}
				};
				api.getData = function () {
					var data = {
						$type: "TOne.WhS.RouteSync.Ericsson.EricssonSwitchRouteSynchronizerSettings, TOne.WhS.RouteSync.Ericsson",
						NumberOfRetries: $scope.scopeModel.numberOfRetries,
						FaultCodes: $scope.scopeModel.faultCodesA,
					};
					return data;
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsRoutesyncEricssonsettingsEditor', EricssonSWSyncEricssonSettings);

})(app);