app.service("SwitchAPIService", function (BaseAPIService) {

    return ({
        GetFilteredSwitches: GetFilteredSwitches,
        GetSwitchById: GetSwitchById,
        GetSwitches: GetSwitches,
        GetSwitchesToLinkTo: GetSwitchesToLinkTo,
        GetSwitchAssignedDataSources: GetSwitchAssignedDataSources,
        UpdateSwitch: UpdateSwitch,
        AddSwitch: AddSwitch,
        DeleteSwitch: DeleteSwitch
    });

    function GetFilteredSwitches(input) {
        return BaseAPIService.post("/api/Switch/GetFilteredSwitches", input);
    }

    function GetSwitchById(switchId) {
        return BaseAPIService.get("/api/Switch/GetSwitchById", {
            switchId: switchId
        });
    }

    function GetSwitches() {
        return BaseAPIService.get("/api/Switch/GetSwitches");
    }

    function GetSwitchesToLinkTo(switchId) {
        return BaseAPIService.get("/api/Switch/GetSwitchesToLinkTo", {
            switchId: switchId
        });
    }

    function GetSwitchAssignedDataSources() {
        return BaseAPIService.get("/api/Switch/GetSwitchAssignedDataSources");
    }

    function UpdateSwitch(switchObj) {
        return BaseAPIService.post("/api/Switch/UpdateSwitch", switchObj);
    }

    function AddSwitch(switchObj) {
        return BaseAPIService.post("/api/Switch/AddSwitch", switchObj);
    }

    function DeleteSwitch(switchId) {
        return BaseAPIService.get("/api/Switch/DeleteSwitch", {
            switchId: switchId
        });
    }
});
