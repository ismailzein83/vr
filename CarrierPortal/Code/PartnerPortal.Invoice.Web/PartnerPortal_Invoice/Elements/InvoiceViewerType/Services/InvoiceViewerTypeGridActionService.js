
app.service('PartnerPortal_Invoice_InvoiceViewerTypeGridActionService', ['VRModalService', 'UtilsService', 'PartnerPortal_Invoice_InvoiceTypeAPIService', 'VRNotificationService', 'SecurityService', 'PartnerPortal_Invoice_DownloadAttachmentActionAPIService',
    function (VRModalService, UtilsService, PartnerPortal_Invoice_InvoiceTypeAPIService, VRNotificationService, SecurityService, PartnerPortal_Invoice_DownloadAttachmentActionAPIService) {

        var actionTypes = [];
        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }
     
        function registerDownloadAttachmentAction() {
            var actionType = {
                ActionTypeName: "DownloadAttachment",
                actionMethod: function (payload) {
                    return PartnerPortal_Invoice_DownloadAttachmentActionAPIService.DownloadAttachment(payload.invoiceViewerTypeId, payload.invoiceAction.InvoiceViewerTypeGridActionId, payload.invoice.Entity.InvoiceId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };
            registerActionType(actionType);
        }
     
        return ({
            getActionTypeIfExist: getActionTypeIfExist,
            registerDownloadAttachmentAction: registerDownloadAttachmentAction,
        });
    }]);
