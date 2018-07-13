app.run(['VR_Voucher_VoucherGenerationActionService','VR_Voucher_VoucherActionService',
function (VR_Voucher_VoucherGenerationActionService,VR_Voucher_VoucherActionService) {

    VR_Voucher_VoucherGenerationActionService.registerActivateAction();
        VR_Voucher_VoucherActionService.registerUnlockAction();

}]);
