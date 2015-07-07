app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetNormalCDRs: GetNormalCDRs,
        GetNumberProfiles: GetNumberProfiles
    });

    function GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn) {
        return BaseAPIService.get("/api/Strategy/GetNormalCDRs",
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
        return BaseAPIService.get("/api/Strategy/GetNumberProfiles",
        {
            fromRow: fromRow,
            toRow: toRow,
            fromDate: fromDate,
            toDate: toDate,
            subscriberNumber: subscriberNumber
        }
       );
    }

    function GetFilteredSuspiciousNumbers(fromRow, toRow, fromDate, toDate, strategyId, suspicionLevelsList) {

        alert('fix here GetFilteredSuspiciousNumbers (Service)')

        return BaseAPIService.get("/api/Strategy/GetFilteredSuspiciousNumbers",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                strategyId: 4,
                suspicionLevelsList: '4,3,2',
            }
           );
    }


   




});