'use strict';

app.directive('vrWhsBePurchaseareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var purchaseAreaSettings = new PurchaseAreaSettings(ctrl, $scope, $attrs);
			purchaseAreaSettings.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PurchaseAreaSettingsTemplate.html"
	};

	function PurchaseAreaSettings(ctrl, $scope, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			defineAPI();
		}
		function defineAPI() {
			var api = {};

			api.load = function (payload) {
				if (payload != undefined && payload.data != undefined) {
					ctrl.retroactiveDayOffset = payload.data.RetroactiveDayOffset;
				}
			};

			api.getData = function () {
				return {
					$type: "TOne.WhS.BusinessEntity.Entities.PurchaseAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
					RetroactiveDayOffset: ctrl.retroactiveDayOffset
				};
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}
}]);