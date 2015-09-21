app.service('CustomerInvoiceAPIService', function (BaseAPIService, DataRetrievalResultTypeEnum) {
    return ({
        GetFilteredCustomerInvoices: GetFilteredCustomerInvoices,
    });

    function GetFilteredCustomerInvoices(input) {
        return BaseAPIService.post('/api/CustomerInvoice/GetFilteredCustomerInvoices', input);
    }

});