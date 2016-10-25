(function (appControllers) {

	'use strict';

	SwapDealAnalysisService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

	function SwapDealAnalysisService(VRModalService, VRNotificationService, UtilsService) {

		function addOutbound(carrierAccountId, onOutboundAdded)
		{
			var parameters = {
				carrierAccountId: carrierAccountId
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onOutboundAdded = onOutboundAdded;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/OutboundEditor.html', parameters, settings);
		}

		function editOutbound(carrierAccountId, outboundEntity, onOutboundUpdated) {
			var parameters = {
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
			addOutbound: addOutbound,
			editOutbound: editOutbound
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisService', SwapDealAnalysisService);

})(appControllers);