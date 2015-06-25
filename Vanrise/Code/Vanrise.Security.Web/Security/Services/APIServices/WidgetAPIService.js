app.service('WidgetAPIService', function (BaseAPIService) {

    return ({ 
        GetWidgetsDefinition: GetWidgetsDefinition,
        SaveWidget: SaveWidget,
        GetAllWidgets: GetAllWidgets
    });

    function GetWidgetsDefinition() {
        return BaseAPIService.get("/api/Widgets/GetWidgetsDefinition");
    }
    function SaveWidget(widget) {
        return BaseAPIService.post("/api/Widgets/SaveWidget", widget);
    }
    function GetAllWidgets() {
        return BaseAPIService.get("/api/Widgets/GetAllWidgets");
    }
   
});