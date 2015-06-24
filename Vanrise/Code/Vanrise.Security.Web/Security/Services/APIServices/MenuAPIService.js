app.service('MenuAPIService', function (BaseAPIService) {

    return ({
        GetMenuItems: GetMenuItems,
        GetAllMenuItems:GetAllMenuItems
    });

    function GetMenuItems() {
        return BaseAPIService.get("/api/Menu/GetMenuItems");
    }
    function GetAllMenuItems() {
        return BaseAPIService.get("/api/Menu/GetAllMenuItems");
    }
});