app.service('DynamicPageAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
        UpdateView:UpdateView,
        SaveView: SaveView,
        GetView: GetView
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/View/GetDynamicPages");
    }
    function SaveView(view) {
        return BaseAPIService.post("/api/View/SaveView", view);
    }
    function UpdateView(view) {
        return BaseAPIService.post("/api/View/UpdateView", view);
    }
    function GetView(ViewId) {
        return BaseAPIService.get("/api/View/GetView",
            {
                ViewId: ViewId
            });
       
    }
});