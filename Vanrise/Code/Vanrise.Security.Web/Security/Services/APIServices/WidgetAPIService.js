app.service('WidgetAPIService', function (BaseAPIService) {

    return ({ 
        GetWidgetsDefinition: GetWidgetsDefinition,
        SaveWidget: SaveWidget,
        GetAllWidgets: GetAllWidgets,
        UpdateWidget: UpdateWidget
    });

    function GetWidgetsDefinition() {
        return BaseAPIService.get("/api/Widgets/GetWidgetsDefinition");
    }
    function SaveWidget(widget) {
        return BaseAPIService.post("/api/Widgets/SaveWidget", widget);
    }
    function UpdateWidget(widget) {
        return BaseAPIService.post("/api/Widgets/UpdateWidget", widget);
    }
    function GetAllWidgets() {
        return BaseAPIService.get("/api/Widgets/GetAllWidgets");
    }
   
});