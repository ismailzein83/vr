
app.service('WhS_SupPL_SupplierPriceListAPIService', function (BaseAPIService) {

    return ({

        UploadSupplierPriceList: UploadSupplierPriceList,
        DownloadSupplierPriceList: DownloadSupplierPriceList
    });


    function UploadSupplierPriceList(supplierAccountId,currencyId, fileId, effectiveDate) {
        return BaseAPIService.get("/api/SupplierPriceList/UploadSupplierPriceList", {
            supplierAccountId: supplierAccountId,
            currencyId:currencyId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

    function DownloadSupplierPriceList() {
        return BaseAPIService.get("/api/SupplierPriceList/DownloadSupplierPriceList", {},{
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }

});