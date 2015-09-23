
app.service('WhS_SupPL_SupplierPriceListAPIService', function (BaseAPIService) {

    return ({

        UploadSaleZonesList: UploadSaleZonesList,
    });


    function UploadSaleZonesList(supplierAccountId, fileId, effectiveDate) {
        return BaseAPIService.get("/api/SupplierPriceList/UploadSaleZonesList", {
            supplierAccountId: supplierAccountId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

});