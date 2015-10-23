
app.service('WhS_BE_SellingNumberPlanAPIService', function (BaseAPIService) {

    return ({

        GetSellingNumberPlans: GetSellingNumberPlans,
    });


    function GetSellingNumberPlans() {
        return BaseAPIService.get("/api/SellingNumberPlan/GetSellingNumberPlans");
    }

});