app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        SaveSubscriberCase: SaveSubscriberCase
    });

    function SaveSubscriberCase(subscriberCase) {
        return BaseAPIService.post("/api/CaseManagement/SaveSubscriberCase",
            subscriberCase
           );
    }


});