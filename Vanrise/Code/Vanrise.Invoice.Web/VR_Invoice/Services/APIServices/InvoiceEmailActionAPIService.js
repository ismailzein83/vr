(function (appControllers) {

    "use strict";
    invoiceEmailActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceEmailActionAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceEmailAction';

        function SendEmail(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "SendEmail"), input);
        }

        function GetEmailTemplate(invoiceId, invoiceMailTemplateId, invoiceActionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetEmailTemplate'), {
                invoiceId: invoiceId,
                invoiceMailTemplateId: invoiceMailTemplateId,
                invoiceActionId: invoiceActionId
            });
        }
        function GetSendEmailAttachmentTypeConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetSendEmailAttachmentTypeConfigs'));
        }
        function DownloadAttachment(invoiceId, attachmentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'DownloadAttachment'), {
                invoiceId: invoiceId,
                attachmentId: attachmentId
            }, {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
        }
        return ({
            SendEmail: SendEmail,
            GetEmailTemplate: GetEmailTemplate,
            GetSendEmailAttachmentTypeConfigs: GetSendEmailAttachmentTypeConfigs,
            DownloadAttachment: DownloadAttachment
        });
    }

    appControllers.service('VR_Invoice_InvoiceEmailActionAPIService', invoiceEmailActionAPIService);

})(appControllers);