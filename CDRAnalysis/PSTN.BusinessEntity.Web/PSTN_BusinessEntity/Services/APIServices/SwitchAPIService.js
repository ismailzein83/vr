app.service("SwitchAPIService", function (BaseAPIService) {

    return ({
        GetSwitchTypes: GetSwitchTypes,
        GetFilteredSwitches: GetFilteredSwitches
    });

    function GetSwitchTypes() {
        return BaseAPIService.get("/api/Switch/GetSwitchTypes");
    }

    function GetFilteredSwitches(input) {
        return BaseAPIService.post("/api/Switch/GetFilteredSwitches", input);
    }
});
