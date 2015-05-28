
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({
        GetTest: GetTest
    });

    function GetTest(name){
        return BaseAPIService.get("/api/BillingStatistics/GetTest",
            {
                name: name
            }
           );
    }

});