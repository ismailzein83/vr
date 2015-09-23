﻿app.service("SwitchAPIService", function (BaseAPIService) {

    return ({
        GetSwitchTypes: GetSwitchTypes,
        GetFilteredSwitches: GetFilteredSwitches,
        GetSwitchByID: GetSwitchByID,
        GetSwitches: GetSwitches,
        GetSwitchesToLinkTo: GetSwitchesToLinkTo,
        UpdateSwitch: UpdateSwitch,
        AddSwitch: AddSwitch,
        DeleteSwitch: DeleteSwitch
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

    function GetSwitches() {
        return BaseAPIService.get("/api/Switch/GetSwitches");
    }

    function GetSwitchesToLinkTo(switchID) {
        return BaseAPIService.get("/api/Switch/GetSwitchesToLinkTo", {
            switchID: switchID
        });
    }

    function UpdateSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/UpdateSwitch", switchObject);
    }

    function AddSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/AddSwitch", switchObject);
    }

    function DeleteSwitch(switchID) {
        return BaseAPIService.get("/api/Switch/DeleteSwitch", {
            switchID: switchID
        });
    }
});
