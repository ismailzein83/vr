app.service('ViewAPIService', function (BaseAPIService) {

    return ({
        UpdateView:UpdateView,
        AddView: AddView,
        DeleteView:DeleteView,
        GetView: GetView,
        GetFilteredDynamicPages: GetFilteredDynamicPages,
        UpdateViewsRank :UpdateViewsRank 
    });

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
        return BaseAPIService.post("/api/View/GetFilteredDynamicViews", filter);
    }
    function UpdateViewsRank(updatedIds) {
        return BaseAPIService.post("/api/View/UpdateViewsRank", updatedIds);
    }
});