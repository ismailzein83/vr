(function (appControllers) {

    'use strict';

    DownloadAttachmentActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function DownloadAttachmentActionAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {
        var controllerName = 'DownloadAttachmentAction';

        function DownloadAttachment(invoiceViewerTypeId, invoiceViewerTypeGridActionId, invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "DownloadAttachment"), {
                invoiceViewerTypeId: invoiceViewerTypeId,
                invoiceViewerTypeGridActionId: invoiceViewerTypeGridActionId,
                invoiceId: invoiceId
            }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        };

        return {
            DownloadAttachment: DownloadAttachment
        };
    }

    appControllers.service('PartnerPortal_Invoice_DownloadAttachmentActionAPIService', DownloadAttachmentActionAPIService);

})(appControllers);