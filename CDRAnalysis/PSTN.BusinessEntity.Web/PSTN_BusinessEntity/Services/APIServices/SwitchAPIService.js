app.service("SwitchAPIService", function (BaseAPIService) {

    return ({
        GetSwitchTypes: GetSwitchTypes,
        GetFilteredSwitches: GetFilteredSwitches,
        GetSwitchByID: GetSwitchByID,
        UpdateSwitch: UpdateSwitch,
        AddSwitch: AddSwitch
    });

    function GetSwitchTypes() {
        return BaseAPIService.get("/api/Switch/GetSwitchTypes");
    }

    function GetFilteredSwitches(input) {
        return BaseAPIService.post("/api/Switch/GetFilteredSwitches", input);
    }

    function GetSwitchByID(switchID) {
        return BaseAPIService.get("/api/Switch/GetSwitchByID", {
            switchID: switchID
        });
    }

    function UpdateSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/UpdateSwitch", switchObject);
    }

    function AddSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/AddSwitch", switchObject);
    }
});
