app.service("SwitchTrunkAPIService", function (BaseAPIService) {

    return ({
        GetFilteredSwitchTrunks: GetFilteredSwitchTrunks,
        GetSwitchTrunkByID: GetSwitchTrunkByID,
        AddSwitchTrunk: AddSwitchTrunk,
        UpdateSwitchTrunk: UpdateSwitchTrunk
    });

    function GetFilteredSwitchTrunks(input) {
        return BaseAPIService.post("/api/SwitchTrunk/GetFilteredSwitchTrunks", input);
    }

    function GetSwitchTrunkByID(trunkID) {
        return BaseAPIService.get("/api/SwitchTrunk/GetSwitchTrunkByID", {
            trunkID: trunkID
        });
    }

    function AddSwitchTrunk(trunkObject) {
        return BaseAPIService.post("/api/SwitchTrunk/AddSwitchTrunk", trunkObject);
    }

    function UpdateSwitchTrunk(trunkObject) {
        return BaseAPIService.post("/api/SwitchTrunk/UpdateSwitchTrunk", trunkObject);
    }
});
