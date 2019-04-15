
app.service('Retail_Interconnect_InvoiceService', ['VRModalService', 'UtilsService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'SecurityService', 'VR_Invoice_InvoiceActionService',
    function (VRModalService, UtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, SecurityService, VR_Invoice_InvoiceActionService) {


        function registerCompareAction() {
            var actionType = {
                ActionTypeName: "CompareInvoiceAction",
                actionMethod: function (payload) {
                    openCompareAction(payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceTypeId, payload.invoiceAction.Settings.InvoiceCarrierType, payload.invoice.Entity.PartnerId);

                }
            };
            VR_Invoice_InvoiceActionService.registerActionType(actionType);
        }

        function openCompareAction(invoiceId, invoiceActionId, invoiceTypeId, invoiceCarrierType, partnerId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceActionId: invoiceActionId,
                invoiceTypeId: invoiceTypeId,
                invoiceCarrierType: invoiceCarrierType
                ,partnerId: partnerId

            };
            VRModalService.showModal('/Client/Modules/Retail_Interconnect/Views/InvoiceCompareTemplate.html', parameters, settings);
        }

        function registerOriginalInvoiceData() {
            var actionType = {
                ActionTypeName: "OriginalInvoiceData",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    var onOriginalInvoiceDataUpdated = function (data) {
                        promiseDeffered.resolve(data);
                    };
                    openOriginalInvoiceData(onOriginalInvoiceDataUpdated, payload.invoice.Entity.InvoiceId, payload.invoice.Entity.InvoiceTypeId);
                    return promiseDeffered.promise;
                }
            };
            VR_Invoice_InvoiceActionService.registerActionType(actionType);
        }

        function openOriginalInvoiceData(onOriginalInvoiceDataUpdated, invoiceId, invoiceTypeId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onOriginalInvoiceDataUpdated = onOriginalInvoiceDataUpdated;
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceTypeId: invoiceTypeId
            };
            VRModalService.showModal('/Client/Modules/Retail_Interconnect/Views/OriginalInvoiceDataTemplate.html', parameters, settings);
        }

        return ({
            registerCompareAction: registerCompareAction,
            registerOriginalInvoiceData: registerOriginalInvoiceData
        });
    }]);
