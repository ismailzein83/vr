
app.service('VR_Invoice_GenericInvoiceService', ['VRModalService',
    function (VRModalService) {


        function generateInvoice(onGenerateInvoice, invoiceTypeId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/GenerateInvoiceEditor.html', parameters, settings);
        }

        return ({
            generateInvoice: generateInvoice
        });
    }]);
