(function (app) {

	'use strict';

	whsRoutesyncEricssonbranchroutesettingsRORange.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonBranchRouteSettingsAPIService'];

	function whsRoutesyncEricssonbranchroutesettingsRORange(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonBranchRouteSettingsAPIService) {
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
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/BranchRoutes/Templates/EricssonSwitchBranchRoutesSettingsRORangeTemplate.html"

		};
		function BranchRouteSettingsCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.brList = [];

				$scope.scopeModel.validateBranchRoutes = function () {
					if ($scope.scopeModel.brList == undefined || $scope.scopeModel.brList.length < 1)
						return "At least one branch route should be added.";
					return null;
				};

				$scope.scopeModel.isEmpty = function (value) {
					return (value == undefined || value == null || value == '');
				};

				$scope.scopeModel.validateNewBranchRoute = function () {
					if (!$scope.scopeModel.isEmpty($scope.scopeModel.brTo) && parseInt($scope.scopeModel.brFrom, 10) >= parseInt($scope.scopeModel.brTo, 10))
						return "To value should be greater than From value.";

					else {
						for (var i = 0; i < $scope.scopeModel.brList.length; i++) {
							var item = $scope.scopeModel.brList[i];
							if (parseInt($scope.scopeModel.brFrom, 10) >= parseInt(item.From, 10) && (!$scope.scopeModel.isEmpty(item.To) && parseInt($scope.scopeModel.brFrom, 10) <= parseInt(item.To, 10)))
								return "Branch route overlapped with existing one.";

							else if (!$scope.scopeModel.isEmpty($scope.scopeModel.brTo) && parseInt($scope.scopeModel.brTo, 10) >= parseInt(item.From, 10) && (!$scope.scopeModel.isEmpty(item.To) && parseInt($scope.scopeModel.brTo, 10) <= parseInt(item.To, 10)))
								return "Branch route overlapped with existing one.";

							else if (parseInt($scope.scopeModel.brFrom, 10) <= parseInt(item.From, 10) && !$scope.scopeModel.isEmpty($scope.scopeModel.brTo) && parseInt($scope.scopeModel.brTo, 10) >= parseInt(item.To, 10))
								return "Branch route overlapped with existing one.";

							else if (parseInt($scope.scopeModel.brFrom, 10) == parseInt(item.From, 10))
								return "Branch route overlapped with existing one.";
						}
					}
				};

				$scope.scopeModel.isBRValid = function () {
					if ($scope.scopeModel.isEmpty($scope.scopeModel.brFrom))
						return false;

					return ($scope.scopeModel.validateNewBranchRoute() == undefined);
				};

				$scope.scopeModel.addBR = function () {
					var br = {
						From: $scope.scopeModel.brFrom,
						To: $scope.scopeModel.brTo,
						IncludeTrunkAsSwitch: false
					};

					$scope.scopeModel.brList.push(br);
					$scope.scopeModel.brFrom = undefined;
					$scope.scopeModel.brTo = undefined;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					if (payload != null)
						$scope.scopeModel.brList = (payload.branchRouteSettings != undefined && payload.branchRouteSettings.RORangeBranchRoutes != undefined) ? payload.branchRouteSettings.RORangeBranchRoutes : [];

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "TOne.WhS.RouteSync.Ericsson.Entities.RORangeBranchRouteSettings, TOne.WhS.RouteSync.Ericsson",
						RORangeBranchRoutes: $scope.scopeModel.brList
					};
				};

				if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsRoutesyncEricssonbranchroutesettingsRorange', whsRoutesyncEricssonbranchroutesettingsRORange);

})(app);