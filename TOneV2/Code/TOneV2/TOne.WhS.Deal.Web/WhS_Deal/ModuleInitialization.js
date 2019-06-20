app.run(['WhS_Deal_VolumeCommitmentService', 'WhS_Deal_SwapDealService','WhS_Deal_SwapDealAnalysisService',
    function (WhS_Deal_VolumeCommitmentService, WhS_Deal_SwapDealService, WhS_Deal_SwapDealAnalysisService) {
        WhS_Deal_VolumeCommitmentService.registerObjectTrackingDrillDownToVolCommitmentDeal();
        WhS_Deal_VolumeCommitmentService.registerHistoryViewAction();
        WhS_Deal_SwapDealService.registerSwapDealSellRouteRuleViewToSwapDeal();
        WhS_Deal_SwapDealService.registerObjectTrackingDrillDownToSwapDeal();
        WhS_Deal_SwapDealService.registerHistoryViewAction();
        WhS_Deal_SwapDealAnalysisService.registerCreateSwapDealGenericBEAction();
        WhS_Deal_SwapDealAnalysisService.registerViewSwapDealGenericBEAction();
    }]);
