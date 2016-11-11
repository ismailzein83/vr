(function (appControllers) {

	'use strict';

	SwapDealAnalysisService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

	function SwapDealAnalysisService(VRModalService, VRNotificationService, UtilsService) {

		function addInbound(settings, carrierAccountId, sellingNumberPlanId, onInboundAdded) {
			var parameters = {
				settings: settings,
				carrierAccountId: carrierAccountId,
				sellingNumberPlanId: sellingNumberPlanId
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onInboundAdded = onInboundAdded;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/InboundEditor.html', parameters, settings);
		}

		function editInbound(settings, carrierAccountId, sellingNumberPlanId, inboundEntity, onInboundUpdated) {
			var parameters = {
				settings: settings,
				carrierAccountId: carrierAccountId,
				sellingNumberPlanId: sellingNumberPlanId,
				inboundEntity: inboundEntity
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onInboundUpdated = onInboundUpdated;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/InboundEditor.html', parameters, settings);
		}

		function addOutbound(settings, carrierAccountId, onOutboundAdded)
		{
			var parameters = {
				settings: settings,
				carrierAccountId: carrierAccountId
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onOutboundAdded = onOutboundAdded;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/OutboundEditor.html', parameters, settings);
		}

		function editOutbound(settings, carrierAccountId, outboundEntity, onOutboundUpdated) {
			var parameters = {
				settings: settings,
				carrierAccountId: carrierAccountId,
				outboundEntity: outboundEntity
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onOutboundUpdated = onOutboundUpdated;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/OutboundEditor.html', parameters, settings);
		}

		return {
			addInbound: addInbound,
			editInbound: editInbound,
			addOutbound: addOutbound,
			editOutbound: editOutbound
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisService', SwapDealAnalysisService);

})(appControllers);