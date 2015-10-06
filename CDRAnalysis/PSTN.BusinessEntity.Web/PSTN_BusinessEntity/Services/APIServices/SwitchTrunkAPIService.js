app.service("SwitchTrunkAPIService", function (BaseAPIService) {

    return ({
        GetFilteredSwitchTrunks: GetFilteredSwitchTrunks,
        GetSwitchTrunkByID: GetSwitchTrunkByID,
        GetSwitchTrunksBySwitchID: GetSwitchTrunksBySwitchID,
        GetSwitchTrunks: GetSwitchTrunks,
        AddSwitchTrunk: AddSwitchTrunk,
        UpdateSwitchTrunk: UpdateSwitchTrunk,
        DeleteSwitchTrunk: DeleteSwitchTrunk,
        LinkToTrunk: LinkToTrunk
    });

    function GetFilteredSwitchTrunks(input) {
        return BaseAPIService.post("/api/SwitchTrunk/GetFilteredSwitchTrunks", input);
    }

    function GetSwitchTrunkByID(trunkID) {
        return BaseAPIService.get("/api/SwitchTrunk/GetSwitchTrunkByID", {
            trunkID: trunkID
        });
    }

    function GetSwitchTrunksBySwitchID(switchID) {
        return BaseAPIService.get("/api/SwitchTrunk/GetSwitchTrunksBySwitchID", {
            switchID: switchID
        });
    }

    function GetSwitchTrunks() {
        return BaseAPIService.get("/api/SwitchTrunk/GetSwitchTrunks");
    }

    function AddSwitchTrunk(trunkObject) {
        return BaseAPIService.post("/api/SwitchTrunk/AddSwitchTrunk", trunkObject);
    }

    function UpdateSwitchTrunk(trunkObject) {
        return BaseAPIService.post("/api/SwitchTrunk/UpdateSwitchTrunk", trunkObject);
    }

    function DeleteSwitchTrunk(trunkID, linkedToTrunkID) {
        return BaseAPIService.get("/api/SwitchTrunk/DeleteSwitchTrunk", {
            trunkID: trunkID,
            linkedToTrunkID: linkedToTrunkID
        });
    }

    function LinkToTrunk(switchTrunkID, linkedToTrunkID) {
        return BaseAPIService.get("/api/SwitchTrunk/LinkToTrunk", {
            switchTrunkID: switchTrunkID,
            linkedToTrunkID: linkedToTrunkID
        });
    }
});
