
app.service('WhS_CodePrep_CodePrepAPIService', function (BaseAPIService) {

    return ({

        UploadSaleZonesList: UploadSaleZonesList,
    });
     

    function UploadSaleZonesList(sellingNumberPlanId, fileId,effectiveDate) {
        return BaseAPIService.get("/api/CodePreparation/UploadSaleZonesList", {
            sellingNumberPlanId: sellingNumberPlanId,
            fileId: fileId,
            effectiveDate: effectiveDate
        });
    }

});