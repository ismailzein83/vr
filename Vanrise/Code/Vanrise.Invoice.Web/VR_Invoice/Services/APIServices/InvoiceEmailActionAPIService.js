(function (appControllers) {

    "use strict";
    invoiceEmailActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceEmailActionAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceEmailAction';

        function SendEmail(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "SendEmail"), input);
        }

        function GetEmailTemplate(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetEmailTemplate'), {
                invoiceId: invoiceId,
            });
        }
        function GetSendEmailAttachmentTypeConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetSendEmailAttachmentTypeConfigs'));
        }
        return ({
            SendEmail: SendEmail,
            GetEmailTemplate: GetEmailTemplate,
            GetSendEmailAttachmentTypeConfigs: GetSendEmailAttachmentTypeConfigs
        });
    }

    appControllers.service('VR_Invoice_InvoiceEmailActionAPIService', invoiceEmailActionAPIService);

})(appControllers);