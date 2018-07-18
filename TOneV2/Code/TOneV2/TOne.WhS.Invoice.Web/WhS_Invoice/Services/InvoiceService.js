
app.service('WhS_Invoice_InvoiceService', ['VRModalService', 'UtilsService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'SecurityService', 'VR_Invoice_InvoiceActionService',
    function (VRModalService, UtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, SecurityService, VR_Invoice_InvoiceActionService) {

      
        function registerCompareAction() {
            var actionType = {
                ActionTypeName: "CompareInvoiceAction",
                actionMethod: function (payload) {
                    //console.log(payload)
                    openCompareAction(payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceTypeId, payload.invoiceAction.Settings.InvoiceCarrierType);
                }
            };
            VR_Invoice_InvoiceActionService.registerActionType(actionType);
        }

        function openCompareAction(invoiceId, invoiceActionId, invoiceTypeId,invoiceCarrierType) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceActionId: invoiceActionId,
                invoiceTypeId: invoiceTypeId,
                invoiceCarrierType:invoiceCarrierType
            };
            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/InvoiceCompareTemplate.html', parameters, settings);
        }


        function registerOriginalInvoiceData() {
            var actionType = {
                ActionTypeName: "OriginalInvoiceData",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    var onOriginalInvoiceDataUpdated = function (data) {
                        promiseDeffered.resolve(data);
                    };
                    openOriginalInvoiceData(onOriginalInvoiceDataUpdated, payload.invoice.Entity.InvoiceId, payload.invoice.Entity.InvoiceTypeId,payload.invoiceAction.Settings.InvoiceCarrierType);
                    return promiseDeffered.promise;
                }
            };
            VR_Invoice_InvoiceActionService.registerActionType(actionType);
        }

        function openOriginalInvoiceData(onOriginalInvoiceDataUpdated, invoiceId, invoiceTypeId,invoiceCarrierType) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onOriginalInvoiceDataUpdated = onOriginalInvoiceDataUpdated;
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceTypeId: invoiceTypeId,
                invoiceCarrierType:invoiceCarrierType
            };
            VRModalService.showModal('/Client/Modules/WhS_Invoice/Views/OriginalInvoiceDataTemplate.html', parameters, settings);
        }

        return ({
            registerCompareAction: registerCompareAction,
            registerOriginalInvoiceData: registerOriginalInvoiceData
        });
    }]);
