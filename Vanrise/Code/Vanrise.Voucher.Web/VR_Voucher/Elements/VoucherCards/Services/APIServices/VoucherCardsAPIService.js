(function (appControllers) {

    "use strict";
    voucherCardsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Voucher_ModuleConfig'];

    function voucherCardsAPIService(BaseAPIService, UtilsService, VR_Voucher_ModuleConfig) {

        var controllerName = 'VoucherCards';

        function ActivateVoucherCards(VoucherCardsActivationInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Voucher_ModuleConfig.moduleName, controllerName, "ActivateVoucherCards"), VoucherCardsActivationInput);
        }

       
        return ({
            ActivateVoucherCards: ActivateVoucherCards
        });
    }

    appControllers.service('VR_Voucher_VoucherCardsAPIService', voucherCardsAPIService);

})(appControllers);