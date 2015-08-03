app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetSubscriberThresholds: GetSubscriberThresholds,
        GetFraudResult: GetFraudResult
    });


    function GetSubscriberThresholds(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetSubscriberThresholds", input);
    }




    function GetFilteredSuspiciousNumbers(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetFilteredSuspiciousNumbers", input);
    }


    function GetFraudResult(fromDate, toDate, strategiesList, suspicionLevelsList, subscriberNumber) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetFraudResult",
            {
                fromDate: fromDate,
                toDate: toDate,
                strategiesList: strategiesList,
                suspicionLevelsList: suspicionLevelsList,
                subscriberNumber: subscriberNumber
            }
           );
    }







});