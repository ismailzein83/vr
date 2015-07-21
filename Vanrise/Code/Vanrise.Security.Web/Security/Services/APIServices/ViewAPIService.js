app.service('ViewAPIService', function (BaseAPIService) {

    return ({
        GetDynamicPages: GetDynamicPages,
        UpdateView:UpdateView,
        AddView: AddView,
        DeleteView:DeleteView,
        GetView: GetView,
        GetFilteredDynamicPages: GetFilteredDynamicPages
    });

    function GetDynamicPages() {
        return BaseAPIService.get("/api/View/GetDynamicPages");
    }
    function AddView(view) {
        return BaseAPIService.post("/api/View/AddView", view);
    }
    function UpdateView(view) {
        return BaseAPIService.post("/api/View/UpdateView", view);
    }
    function DeleteView(viewId) {
        return BaseAPIService.get("/api/View/DeleteView", {
            viewId: viewId
            });
    }
    function GetView(ViewId) {
        return BaseAPIService.get("/api/View/GetView",
            {
                ViewId: ViewId
            });
       
    }
    function GetFilteredDynamicPages(filter) {
        return BaseAPIService.get("/api/View/GetFilteredDynamicViews",
                   {
                       filter: filter
                   });
    }
});