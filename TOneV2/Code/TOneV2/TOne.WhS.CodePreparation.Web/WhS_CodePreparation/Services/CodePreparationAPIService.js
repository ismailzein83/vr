
app.service('WhS_CodePrep_CodePrepAPIService', function (BaseAPIService) {

    return ({

        UploadSaleZonesList: UploadSaleZonesList,
    });
     

    function UploadSaleZonesList(saleZonePackageId, fileId,effectiveDate) {
        return BaseAPIService.get("/api/CodePreparation/UploadSaleZonesList", {
            saleZonePackageId: saleZonePackageId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

});