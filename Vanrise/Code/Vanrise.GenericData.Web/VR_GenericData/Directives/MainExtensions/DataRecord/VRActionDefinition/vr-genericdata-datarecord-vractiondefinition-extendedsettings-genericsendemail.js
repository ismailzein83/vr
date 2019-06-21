app.directive('vrGenericdataDatarecordVractiondefinitionExtendedsettingsGenericsendemail', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailMessageTypeAPIService',
	function (UtilsService, VRUIUtilsService, VRCommon_VRMailMessageTypeAPIService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new genericSendEmailActionDefinition($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecord/VRActionDefinition/Templates/VRActionDefinitionGenericSendEmail.html'
		};

		function genericSendEmailActionDefinition($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var beDefinitionSelectorAPI;
			var beDefinitionSelectorDeferredReady = UtilsService.createPromiseDeferred();

			var mailMessageTypeSelectorAPI;
			var mailMessageTypeSelectorDeferredReady = UtilsService.createPromiseDeferred();
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
					beDefinitionSelectorAPI = api;
					beDefinitionSelectorDeferredReady.resolve();
				};
				$scope.scopeModel.onMailMessageTemplateSelectorReady = function (api) {
					mailMessageTypeSelectorAPI = api;
					mailMessageTypeSelectorDeferredReady.resolve();
				};
				defineAPI();
			};

			function defineAPI() {
				var api = {};
				api.load = function (payload) {
					var promises = [];
					var beDefinitionId;
					var mailMessageTypeId;
					if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings) {
						beDefinitionId = payload.Settings.ExtendedSettings.FinancialAccountBEDefinitionId;
						mailMessageTypeId = payload.Settings.ExtendedSettings.MailMessageTypeId;;
					}
					promises.push(loadBEDefinitionSelector());
					promises.push(loadMailMessageTemplateSelector());

					function loadBEDefinitionSelector() {
						var selectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
						beDefinitionSelectorDeferredReady.promise.then(function () {
							var selectorPayload = { selectedIds: beDefinitionId };
							VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, selectorLoadPromiseDeferred);
						});
						return selectorLoadPromiseDeferred.promise;
					}

					function loadMailMessageTemplateSelector() {
						var selectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
						beDefinitionSelectorDeferredReady.promise.then(function () {
							var selectorPayload = { selectedIds: mailMessageTypeId };
							VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, selectorPayload, selectorLoadPromiseDeferred);
						});
						return selectorLoadPromiseDeferred.promise;
					}
					return UtilsService.waitMultiplePromises(promises);
				};
				api.getData = function () {
					return {
						$type: 'Vanrise.AccountBalance.MainExtensions.GenericSendEmailActionDefinitionSettings,Vanrise.AccountBalance.MainExtensions',
						FinancialAccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
						MailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds()
					};
				};
				if (ctrl.onReady != null)
					ctrl.onReady(api);
			};
		}
	}]);