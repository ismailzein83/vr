(function (appControllers) {

	'use strict';

	SwapDealAnalysisSettingsService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

	function SwapDealAnalysisSettingsService(VRModalService, VRNotificationService, UtilsService) {

		var outboundRateCalcMethodEditorUrl = '/Client/Modules/WhS_Deal/Views/SwapDealAnalysisSettings/Templates/OutboundRateCalcMethodEditor.html';

		function addOutboundRateCalcMethod(onOutboundRateCalcMethodAdded) {

			var parameters = null;

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onOutboundRateCalcMethodAdded = onOutboundRateCalcMethodAdded;
			};

			VRModalService.showModal(outboundRateCalcMethodEditorUrl, parameters, settings);
		}

		function editOutboundRateCalcMethod(calculationMethodId, outboundRateCalcMethod, onOutboundRateCalcMethodUpdated) {

			var parameters = {
				calculationMethodId: calculationMethodId,
				outboundRateCalcMethod: outboundRateCalcMethod
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onOutboundRateCalcMethodUpdated = onOutboundRateCalcMethodUpdated;
			};

			VRModalService.showModal(outboundRateCalcMethodEditorUrl, parameters, settings);
		}

		return {
			addOutboundRateCalcMethod: addOutboundRateCalcMethod,
			editOutboundRateCalcMethod: editOutboundRateCalcMethod
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisSettingsService', SwapDealAnalysisSettingsService);

})(appControllers);