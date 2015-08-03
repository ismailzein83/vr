app.service('NumberProfileAPIService', function (BaseAPIService) {

    return ({
        GetNumberProfiles: GetNumberProfiles
    });


    function GetNumberProfiles(input) {
        return BaseAPIService.post("/api/NumberProfile/GetNumberProfiles", input  );
    }


});