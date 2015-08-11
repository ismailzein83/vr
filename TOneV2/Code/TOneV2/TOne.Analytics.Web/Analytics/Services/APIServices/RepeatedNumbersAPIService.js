
app.service('RepeatedNumbersAPIService', function (BaseAPIService) {

    return ({
        GetRepeatedNumbersData: GetRepeatedNumbersData,

    });


    function GetRepeatedNumbersData(repeatedNumbersDataInput) {
        return BaseAPIService.post("/api/RepeatedNumbers/GetRepeatedNumbersData", repeatedNumbersDataInput);
    }


});