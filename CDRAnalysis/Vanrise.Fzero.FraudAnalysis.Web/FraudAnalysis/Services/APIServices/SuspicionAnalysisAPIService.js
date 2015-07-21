app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetNormalCDRs: GetNormalCDRs,
        GetSubscriberThresholds: GetSubscriberThresholds,
        GetNumberProfiles: GetNumberProfiles
    });

    function GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetNormalCDRs",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                msisdn: msisdn
            }
           );
    }


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



    function GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetNumberProfiles",
        {
            fromRow: fromRow,
            toRow: toRow,
            fromDate: fromDate,
            toDate: toDate,
            subscriberNumber: subscriberNumber
        }
       );
    }

    function GetFilteredSuspiciousNumbers(tempTableKey, fromRow, toRow, fromDate, toDate, strategiesList, suspicionLevelsList) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetFilteredSuspiciousNumbers",
            {
                tempTableKey: tempTableKey,
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                strategiesList: strategiesList,
                suspicionLevelsList: suspicionLevelsList,
            }
           );
    }


   




});