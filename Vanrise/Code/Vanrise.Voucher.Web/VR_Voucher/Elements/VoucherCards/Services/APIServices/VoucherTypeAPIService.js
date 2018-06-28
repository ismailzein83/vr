(function (appControllers) {

    "use strict";
    voucherTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Voucher_ModuleConfig', 'SecurityService'];

    function voucherTypeAPIService(BaseAPIService, UtilsService, VR_Voucher_ModuleConfig, SecurityService) {

        var controllerName = 'VoucherCardDefinition';

        function GetVoucherCardDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Voucher_ModuleConfig.moduleName, controllerName, "GetVoucherCardDefinition"));
        }

       
        return ({
            GetVoucherCardDefinition: GetVoucherCardDefinition
        });
    }

    appControllers.service('VR_Voucher_VoucherTypeAPIService', voucherTypeAPIService);

})(appControllers);