(function (appControllers) {

	'use strict';

	SwapDealService.$inject = ['VRModalService', 'VRNotificationService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService'];

	function SwapDealService(VRModalService, VRNotificationService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService)
	{
	    var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealEditor.html';

		function analyzeSwapDeal(onSwapDealAnalyzed)
		{
			var parameters = null;

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onSwapDealAnalyzed = onSwapDealAnalyzed;
			};

			VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/SwapDealAnalysis.html', parameters, settings);
		}

		function addSwapDeal(onSwapDealAdded) {
		    var settings = {};

		    settings.onScopeReady = function (modalScope) {
		        modalScope.onSwapDealAdded = onSwapDealAdded;
		    };
		    VRModalService.showModal(editorUrl, null, settings);
		}

		function editSwapDeal(dealId, onSwapDealUpdated) {
		    var parameters = {
		        dealId: dealId
		    };

		    var settings = {};

		    settings.onScopeReady = function (modalScope) {
		        modalScope.onSwapDealUpdated = onSwapDealUpdated;
		    };

		    VRModalService.showModal(editorUrl, parameters, settings);
		}
		return {
		    analyzeSwapDeal: analyzeSwapDeal,
		    addSwapDeal: addSwapDeal,
		    editSwapDeal: editSwapDeal
		};
	}

	appControllers.service('WhS_Deal_SwapDealService', SwapDealService);

})(appControllers);