app.service("SwitchTypeAPIService", function (BaseAPIService) {

    return ({
        GetSwitchTypes: GetSwitchTypes,
        GetFilteredSwitchTypes: GetFilteredSwitchTypes,
        GetSwitchTypeByID: GetSwitchTypeByID,
        AddSwitchType: AddSwitchType,
        UpdateSwitchType: UpdateSwitchType,
        DeleteSwitchType: DeleteSwitchType
    });

    function GetSwitchTypes() {
        return BaseAPIService.get("/api/SwitchType/GetSwitchTypes");
    }

    function GetFilteredSwitchTypes(input) {
        return BaseAPIService.post("/api/SwitchType/GetFilteredSwitchTypes", input);
    }

    function GetSwitchTypeByID(switchTypeID) {
        return BaseAPIService.get("/api/SwitchType/GetSwitchTypeByID", {
            switchTypeID: switchTypeID
        });
    }

    function AddSwitchType(switchTypeObject) {
        return BaseAPIService.post("/api/SwitchType/AddSwitchType", switchTypeObject);
    }

    function UpdateSwitchType(switchTypeObject) {
        return BaseAPIService.post("/api/SwitchType/UpdateSwitchType", switchTypeObject);
    }

    function DeleteSwitchType(switchTypeID) {
        return BaseAPIService.get("/api/SwitchType/DeleteSwitchType", {
            switchTypeID: switchTypeID
        });
    }
});
