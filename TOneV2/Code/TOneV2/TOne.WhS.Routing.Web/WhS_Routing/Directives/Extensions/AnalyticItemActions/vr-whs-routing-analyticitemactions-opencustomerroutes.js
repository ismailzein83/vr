(function (app) {

	'use strict';

	AnalyticitemactionsOpencustomerroutesDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticReportAPIService'];

	function AnalyticitemactionsOpencustomerroutesDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticReportAPIService) {
		return {
			restrict: "E",
			scope: {
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var analyticitemactionsOpencustomerroutes = new AnalyticitemactionsOpencustomerroutes($scope, ctrl, $attrs);
				analyticitemactionsOpencustomerroutes.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/AnalyticItemActions/Templates/OpenCustomerRoutesItemAction.html'

		};
		function AnalyticitemactionsOpencustomerroutes($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}
			
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
				};

				api.getData = getData;

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}

				function getData() {
					var data = {
						$type: "TOne.WhS.Routing.MainExtensions.AnalyticItemActions.OpenCustomerRoutesItemAction, TOne.WhS.Routing.MainExtensions",
					};
					return data;
				}
			}
		}
	}

	app.directive('vrWhsRoutingAnalyticitemactionsOpencustomerroutes', AnalyticitemactionsOpencustomerroutesDirective);

})(app);