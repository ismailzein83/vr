﻿app.service('WidgetAPIService', function (BaseAPIService) {

    return ({ 
        GetWidgetsDefinition: GetWidgetsDefinition,
        SaveWidget: SaveWidget,
        GetAllWidgets: GetAllWidgets,
        UpdateWidget: UpdateWidget,
        GetFilteredWidgets: GetFilteredWidgets,
        DeleteWidget: DeleteWidget
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
    function GetFilteredWidgets(widgetName, widgetType)
    {
        return BaseAPIService.get("/api/Widgets/GetFilteredWidgets",{
            widgetName: widgetName,
            widgetType: widgetType
            });
    }
    function DeleteWidget(widgetId) {
        return BaseAPIService.get("/api/Widgets/DeleteWidget", {
            widgetId: widgetId
        });
    }
   
});