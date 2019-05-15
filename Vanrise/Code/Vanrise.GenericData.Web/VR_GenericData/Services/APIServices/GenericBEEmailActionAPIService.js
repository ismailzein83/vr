//(function (appControllers) {

//    "use strict";
//    genericBEEmailActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig', 'SecurityService'];

//    function genericBEEmailActionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

//        var controllerName = 'GenericBEEmailAction';

//        function SendEmail(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "SendEmail"), input);
//        }

//        function GetEmailTemplate(invoiceId, invoiceMailTemplateId, invoiceActionId) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetEmailTemplate'), {
//                invoiceId: invoiceId,
//                invoiceMailTemplateId: invoiceMailTemplateId,
//                invoiceActionId: invoiceActionId
//            });
//        }
//        function GetSendEmailAttachmentTypeConfigs()
//        {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetSendEmailAttachmentTypeConfigs'));
//        }
//        function DownloadAttachment(invoiceId, attachmentId) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DownloadAttachment'), {
//                invoiceId: invoiceId,
//                attachmentId: attachmentId
//            }, {
//            returnAllResponseParameters: true,
//            responseTypeAsBufferArray: true
//        });
//        }
//        return ({
//            SendEmail: SendEmail,
//            GetEmailTemplate: GetEmailTemplate,
//            GetSendEmailAttachmentTypeConfigs: GetSendEmailAttachmentTypeConfigs,
//            DownloadAttachment: DownloadAttachment
//        });
//    }

//    appControllers.service('VR_GenericData_GenericBEEmailActionAPIService', genericBEEmailActionAPIService);

//})(appControllers);