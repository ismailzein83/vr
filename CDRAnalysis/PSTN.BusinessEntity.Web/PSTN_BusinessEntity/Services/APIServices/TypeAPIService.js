app.service("TypeAPIService", function (BaseAPIService) {

    return ({
        GetTypes: GetTypes,
        GetFilteredTypes: GetFilteredTypes,
        GetTypeById: GetTypeById,
        AddType: AddType,
        UpdateType: UpdateType,
        DeleteType: DeleteType
    });

    function GetTypes() {
        return BaseAPIService.get("/api/Type/GetTypes");
    }

    function GetFilteredTypes(input) {
        return BaseAPIService.post("/api/Type/GetFilteredTypes", input);
    }

    function GetTypeById(switchTypeId) {
        return BaseAPIService.get("/api/Type/GetTypeById", {
            switchTypeId: switchTypeId
        });
    }

    function AddType(switchTypeObj) {
        return BaseAPIService.post("/api/Type/AddType", switchTypeObj);
    }

    function UpdateType(switchTypeObj) {
        return BaseAPIService.post("/api/Type/UpdateType", switchTypeObj);
    }

    function DeleteType(switchTypeId) {
        return BaseAPIService.get("/api/Type/DeleteType", {
            switchTypeId: switchTypeId
        });
    }
});
