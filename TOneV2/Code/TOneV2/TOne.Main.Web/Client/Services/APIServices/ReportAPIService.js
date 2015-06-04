app.service('ReportAPIService', function (BaseAPIService) {

    return ({
        GetAllReportDefinition: GetAllReportDefinition
    });
    function GetAllReportDefinition() {
        return BaseAPIService.get("api/ReportDefintion/GetAllRDLCReportDefinition");
    }

    

});