app.service('DynamicPagesManagementAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
        GetWidgets: GetWidgets,
        SavePage:SavePage
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetDynamicPages");
    }
    function GetWidgets() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetWidgets");
    }
    function SavePage(PageSettings) {
        return BaseAPIService.post("/api/DynamicPagesManagement/SavePage", PageSettings);
    }
});