(function (app) {

	'use strict';

	whsRoutesyncEricssonbranchroutesettingsFreeText.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonBranchRouteSettingsAPIService'];

	function whsRoutesyncEricssonbranchroutesettingsFreeText(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonBranchRouteSettingsAPIService) {
		return {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new BranchRouteSettingsCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/BranchRoutes/Templates/EricssonSwitchBranchRoutesSettingsFreeTextTemplate.html"

		};
		function BranchRouteSettingsCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.brList = [];

				$scope.scopeModel.validateBranchRouteName = function () {
					if ($scope.scopeModel.brToAdd != undefined && $scope.scopeModel.brToAdd.includes('~'))
						return "Branch Route name can not include '~'.";
				};

				$scope.scopeModel.validateBranchRoutes = function () {
					if ($scope.scopeModel.brList == undefined || $scope.scopeModel.brList.length < 1)
						return "At least one branch route should be added.";
					return null;
				};

				$scope.scopeModel.isBRValid = function () {
					var brIsValid = true;
					if ($scope.scopeModel.brToAdd == undefined || $scope.scopeModel.brToAdd.length == 0) {
						brIsValid = false;
					}
					else {
						angular.forEach($scope.scopeModel.brList, function (item) {
							if ($scope.scopeModel.brToAdd === item.Name) {
								brIsValid = false;
							}
						});
					}
					return brIsValid;
				};

				$scope.scopeModel.addBR = function () {
					var br = {
						Name: $scope.scopeModel.brToAdd,
						IncludeTrunkAsSwitch: false
					};

					$scope.scopeModel.brList.push(br);
					$scope.scopeModel.brToAdd = undefined;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					if (payload != null)
						$scope.scopeModel.brList = (payload.branchRouteSettings != undefined && payload.branchRouteSettings.FreeTextBranchRoutes != undefined) ? payload.branchRouteSettings.FreeTextBranchRoutes : [];

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "TOne.WhS.RouteSync.Ericsson.Entities.FreeTextBranchRouteSettings, TOne.WhS.RouteSync.Ericsson",
						FreeTextBranchRoutes: $scope.scopeModel.brList
					};
				};

				if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsRoutesyncEricssonbranchroutesettingsFreetext', whsRoutesyncEricssonbranchroutesettingsFreeText);

})(app);