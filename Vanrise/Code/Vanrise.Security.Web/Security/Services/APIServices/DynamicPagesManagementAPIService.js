app.service('DynamicPagesManagementAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
        GetWidgets: GetWidgets,
        SaveView: SaveView,
        GetView: GetView
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetDynamicPages");
    }
    function GetWidgets() {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetWidgets");
    }
    function SaveView(view) {
        return BaseAPIService.post("/api/DynamicPagesManagement/SaveView", view);
    }
    function GetView(ViewId) {
        return BaseAPIService.get("/api/DynamicPagesManagement/GetView",
            {
                ViewId: ViewId
            });
       
    }
});