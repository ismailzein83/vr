app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetSubscriberThresholds: GetSubscriberThresholds,
        GetFraudResult: GetFraudResult
    });


    function GetSubscriberThresholds(fromRow, toRow, fromDate, toDate, msisdn) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetSubscriberThresholds",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                msisdn: msisdn
            }
           );
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