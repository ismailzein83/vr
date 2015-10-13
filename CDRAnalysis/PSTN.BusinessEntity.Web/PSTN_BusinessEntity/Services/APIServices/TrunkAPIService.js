app.service("TrunkAPIService", function (BaseAPIService) {

    return ({
        GetFilteredTrunks: GetFilteredTrunks,
        GetTrunkById: GetTrunkById,
        GetTrunksBySwitchIds: GetTrunksBySwitchIds,
        GetTrunks: GetTrunks,
        AddTrunk: AddTrunk,
        UpdateTrunk: UpdateTrunk,
        DeleteTrunk: DeleteTrunk
    });

    function GetFilteredTrunks(input) {
        return BaseAPIService.post("/api/Trunk/GetFilteredTrunks", input);
    }

    function GetTrunkById(trunkId) {
        return BaseAPIService.get("/api/Trunk/GetTrunkById", {
            trunkId: trunkId
        });
    }

    function GetTrunksBySwitchIds(trunkFilterObj) {
        return BaseAPIService.post("/api/Trunk/GetTrunksBySwitchIds", trunkFilterObj);
    }

    function GetTrunks() {
        return BaseAPIService.get("/api/Trunk/GetTrunks");
    }

    function AddTrunk(trunkObj) {
        return BaseAPIService.post("/api/Trunk/AddTrunk", trunkObj);
    }

    function UpdateTrunk(trunkObj) {
        return BaseAPIService.post("/api/Trunk/UpdateTrunk", trunkObj);
    }

    function DeleteTrunk(trunkId, linkedToTrunkId) {
        return BaseAPIService.get("/api/Trunk/DeleteTrunk", {
            trunkId: trunkId,
            linkedToTrunkId: linkedToTrunkId
        });
    }
});
