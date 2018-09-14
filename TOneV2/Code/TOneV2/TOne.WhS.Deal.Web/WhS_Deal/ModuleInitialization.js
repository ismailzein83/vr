app.run(['WhS_Deal_VolumeCommitmentService', 'WhS_Deal_SwapDealService',
function (WhS_Deal_VolumeCommitmentService, WhS_Deal_SwapDealService) {
    WhS_Deal_VolumeCommitmentService.registerObjectTrackingDrillDownToVolCommitmentDeal();
    WhS_Deal_VolumeCommitmentService.registerHistoryViewAction();
   // WhS_Deal_SwapDealService.registerSwapDealSellRouteRuleViewToSwapDeal();
    //WhS_Deal_SwapDealService.registerSwapDealBuyRouteRuleViewToSwapDeal();
    WhS_Deal_SwapDealService.registerObjectTrackingDrillDownToSwapDeal();
    WhS_Deal_SwapDealService.registerHistoryViewAction();
}]);
