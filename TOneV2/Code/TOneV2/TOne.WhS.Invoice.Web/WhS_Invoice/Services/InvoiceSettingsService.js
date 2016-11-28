
app.service('WhS_Invoice_InvoiceSettingService', ['UtilsService',
    function (UtilsService) {
        function getCustomerInvoiceMailType()
        {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            promiseDeffered.resolve("d077a578-53b3-4faf-84b7-5e1ef5724c79");
            return promiseDeffered.promise;
        }
        return ({
            getCustomerInvoiceMailType: getCustomerInvoiceMailType,
        });
    }]);
