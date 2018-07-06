 (function (appControllers) {

    'use strict';

    VoucherCardsGenerationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Voucher_ModuleConfig'];

    function VoucherCardsGenerationAPIService(BaseAPIService, UtilsService, VR_Voucher_ModuleConfig) {
        var controllerName = 'VoucherCardsGeneration';

        return {
            
            GetVoucherCardsGeneration: GetVoucherCardsGeneration
        };

        
        function GetVoucherCardsGeneration(voucherCardsGenerationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Voucher_ModuleConfig.moduleName, controllerName, 'GetVoucherCardsGeneration'), {
                voucherCardsGenerationId: voucherCardsGenerationId
            });
        }
              
     
    }

    appControllers.service('VR_Voucher_VoucherCardsGenerationAPIService', VoucherCardsGenerationAPIService);

})(appControllers);