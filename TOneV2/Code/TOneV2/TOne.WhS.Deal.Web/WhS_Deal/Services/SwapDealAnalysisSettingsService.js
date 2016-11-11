(function (appControllers) {

	'use strict';

	SwapDealAnalysisSettingsService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

	function SwapDealAnalysisSettingsService(VRModalService, VRNotificationService, UtilsService) {

		var inboundRateCalcMethodEditorUrl = '/Client/Modules/WhS_Deal/Views/SwapDealAnalysisSettings/Templates/InboundRateCalcMethodEditor.html';
		var outboundRateCalcMethodEditorUrl = '/Client/Modules/WhS_Deal/Views/SwapDealAnalysisSettings/Templates/OutboundRateCalcMethodEditor.html';

		function addInboundRateCalcMethod(onInboundRateCalcMethodAdded) {

			var parameters = null;

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onInboundRateCalcMethodAdded = onInboundRateCalcMethodAdded;
			};

			VRModalService.showModal(inboundRateCalcMethodEditorUrl, parameters, settings);
		}

		function editInboundRateCalcMethod(calculationMethodId, inboundRateCalcMethod, onInboundRateCalcMethodUpdated) {

			var parameters = {
				calculationMethodId: calculationMethodId,
				inboundRateCalcMethod: inboundRateCalcMethod
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onInboundRateCalcMethodUpdated = onInboundRateCalcMethodUpdated;
			};

			VRModalService.showModal(inboundRateCalcMethodEditorUrl, parameters, settings);
		}

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
			addInboundRateCalcMethod: addInboundRateCalcMethod,
			editInboundRateCalcMethod: editInboundRateCalcMethod,
			addOutboundRateCalcMethod: addOutboundRateCalcMethod,
			editOutboundRateCalcMethod: editOutboundRateCalcMethod
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisSettingsService', SwapDealAnalysisSettingsService);

})(appControllers);