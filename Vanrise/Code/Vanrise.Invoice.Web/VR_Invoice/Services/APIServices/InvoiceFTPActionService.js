//(function (appControllers) {

//    "use strict";
//    invoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

//    function invoiceAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

//        var controllerName = 'InvoiceFTPAction';

//        function SendFTPFile(invoiceActionId, invoiceId, connectionId) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'SendFTPFile'), {
//                invoiceActionId: invoiceActionId,.
//                invoiceId: invoiceId,
//                connectionId: connectionId
//            });
//        }
   
//        return ({
    
//            SendFTPFile: SendFTPFile
//        });
//    }

//    appControllers.service('VR_Invoice_InvoiceFTPAPIService', invoiceAPIService);

//})(appControllers);