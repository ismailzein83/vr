
app.service('WhS_SupPL_SupplierPriceListAPIService', function (BaseAPIService) {

    return ({

        UploadSupplierPriceList: UploadSupplierPriceList,
    });


    function UploadSupplierPriceList(supplierAccountId, fileId, effectiveDate) {
        return BaseAPIService.get("/api/SupplierPriceList/UploadSupplierPriceList", {
            supplierAccountId: supplierAccountId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

});