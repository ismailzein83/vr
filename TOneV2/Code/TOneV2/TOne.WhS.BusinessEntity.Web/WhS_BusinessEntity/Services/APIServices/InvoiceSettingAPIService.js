(function (appControllers) {

    'use strict';

    InvoiceSettingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function InvoiceSettingAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        var controllerName = 'InvoiceSetting';

        function GetInvoiceSettingsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetInvoiceSettingsInfo'));
        }

        return ({
            GetInvoiceSettingsInfo: GetInvoiceSettingsInfo
        });
    }

    appControllers.service('WhS_BE_InvoiceSettingAPIService', InvoiceSettingAPIService);

})(appControllers);