app.service('DynamicPagesManagementAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
        GetWidgets:GetWidgets
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetDynamicPages");
    }
    function GetWidgets() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetWidgets");
    }
});