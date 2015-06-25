app.service('DynamicPagesAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
       
        SaveView: SaveView,
        GetView: GetView
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/DynamicViewsManagement/GetDynamicPages");
    }
    function SaveView(view) {
        return BaseAPIService.post("/api/DynamicViewsManagement/SaveView", view);
    }
    function GetView(ViewId) {
        return BaseAPIService.get("/api/DynamicViewsManagement/GetView",
            {
                ViewId: ViewId
            });
       
    }
});