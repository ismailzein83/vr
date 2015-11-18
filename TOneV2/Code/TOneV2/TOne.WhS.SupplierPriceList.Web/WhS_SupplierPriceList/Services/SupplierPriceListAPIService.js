
app.service('WhS_SupPL_SupplierPriceListAPIService', function (BaseAPIService) {

    return ({

        UploadSupplierPriceList: UploadSupplierPriceList,
    });


    function UploadSupplierPriceList(supplierAccountId,currencyId, fileId, effectiveDate) {
        return BaseAPIService.get("/api/SupplierPriceList/UploadSupplierPriceList", {
            supplierAccountId: supplierAccountId,
            currencyId:currencyId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

});