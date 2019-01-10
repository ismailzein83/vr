(function (app) {
	'use strict';
	SetDefaultSecurityProviderActionDirective.$inject = ['UtilsService', 'VRNotificationService'];
	function SetDefaultSecurityProviderActionDirective(UtilsService, VRNotificationService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new SetDefaultSecurityProviderAction($scope, ctrl);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/Security/Directives/SecurityProvider/MainExtensions/Templates/SetDefaultSecurityProviderActionTemplate.html'
		};

		function SetDefaultSecurityProviderAction($scope, ctrl) {

			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var context = payload.context;
					if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
						context.showSecurityGridCallBack(true);

					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Vanrise.Security.MainExtensions.GenericBEActions.SetDefaultSecurityProviderAction, Vanrise.Security.MainExtensions",
					};
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}
	app.directive('vrSecSecurityproviderSetdefaultsecurityprovider', SetDefaultSecurityProviderActionDirective);
})(app);