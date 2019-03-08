
//app.service('Retail_Interconnect_InvoiceService', ['VRModalService', 'UtilsService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'SecurityService', 'VR_Invoice_InvoiceActionService',
//    function (VRModalService, UtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, SecurityService, VR_Invoice_InvoiceActionService) {


//        function registerCompareAction() {
//            var actionType = {
//                ActionTypeName: "CompareInvoiceAction",
//                actionMethod: function (payload) {
//                    openCompareAction(payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceTypeId, payload.invoiceAction.Settings.InvoiceCarrierType);

//                }
//            };
//            VR_Invoice_InvoiceActionService.registerActionType(actionType);
//        }

//        function openCompareAction(invoiceId, invoiceActionId, invoiceTypeId, invoiceCarrierType, partnerId) {
//            var settings = {

//            };
//            settings.onScopeReady = function (modalScope) {
//            };
//            var parameters = {
//                invoiceId: invoiceId,
//                invoiceActionId: invoiceActionId,
//                invoiceTypeId: invoiceTypeId,
//                invoiceCarrierType: invoiceCarrierType
//                //,partnerId: partnerId

//            };
//            VRModalService.showModal('/Client/Modules/Retail_Interconnect/Views/InvoiceCompareTemplate.html', parameters, settings);
//        }

//        return ({
//            registerCompareAction: registerCompareAction
//        });
//    }]);
