'use strict';

app.directive('vrWhsBePurchaseareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyAPIService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, VRCommon_CurrencyAPIService, VRNotificationService) {
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
			var data;

			function initializeController() {

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					if (payload != undefined) {
						data = payload.data;
					}

					var promises = [];
					load();

					function load() {
						loadAllControls();
					}

					function loadAllControls() {
						return UtilsService.waitMultipleAsyncOperations([loadStaticData, getSystemCurrency])
							.catch(function (error) {
								VRNotificationService.notifyExceptionWithClose(error, $scope);
							})
							.finally(function () {
							});
					}

				};

				api.getData = function () {
				    return {
				        $type: "TOne.WhS.BusinessEntity.Entities.PurchaseAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
				        EffectiveDateDayOffset: ctrl.effectiveDateDayOffset,
				        RetroactiveDayOffset: ctrl.retroactiveDayOffset,
				        MaximumRate: ctrl.maximumRate,
				        MaximumCodeRange: ctrl.maximumCodeRange,
				        AcceptableIncreasedRate: ctrl.acceptableIncreasedRate, 
				        AcceptableDecreasedRate: ctrl.acceptableDecreasedRate,
				        CodeGroupVerfifcation: ctrl.codeGroupVerification
				    };
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function loadStaticData() {
				if (data == undefined)
				    return;
				ctrl.effectiveDateDayOffset = data.EffectiveDateDayOffset;
				ctrl.retroactiveDayOffset = data.RetroactiveDayOffset;
				ctrl.maximumRate = data.MaximumRate;
				ctrl.maximumCodeRange = data.MaximumCodeRange;
			    ctrl.acceptableIncreasedRate = data.AcceptableIncreasedRate;
			    ctrl.acceptableDecreasedRate =  data.AcceptableDecreasedRate;
				ctrl.codeGroupVerification = data.CodeGroupVerfifcation;
			}

			function getSystemCurrency() {
				return VRCommon_CurrencyAPIService.GetSystemCurrency().then(function (response) {
					if (response != undefined) {
						ctrl.systemCurrencySymbol = response.Symbol;
					}
				});
			}
		}
	}]);