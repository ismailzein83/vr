app.service('NumberProfileAPIService', function (BaseAPIService) {

    return ({
        GetNumberProfiles: GetNumberProfiles
    });


    function GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber) {
        return BaseAPIService.get("/api/NumberProfile/GetNumberProfiles",
        {
            fromRow: fromRow,
            toRow: toRow,
            fromDate: fromDate,
            toDate: toDate,
            subscriberNumber: subscriberNumber
        }
       );
    }


});