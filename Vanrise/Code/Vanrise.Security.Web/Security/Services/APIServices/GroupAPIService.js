app.service('GroupAPIService', function (BaseAPIService) {

    return ({
        GetFilteredGroups: GetFilteredGroups,
        GetGroup: GetGroup,
        GetGroups: GetGroups,
        AddGroup: AddGroup,
        UpdateGroup: UpdateGroup
    });

    function GetFilteredGroups(fromRow, toRow, name) {
        return BaseAPIService.get("/api/Group/GetFilteredGroups", {
            fromRow: fromRow,
            toRow: toRow,
            name: name
        });
    }

    function GetGroup(groupId) {
        return BaseAPIService.get("/api/Group/GetGroup", {
            groupId: groupId
        });
    }

    function GetGroups() {
        return BaseAPIService.get("/api/Group/GetGroups");
    }

    function AddGroup(group) {
        return BaseAPIService.post("/api/Group/AddGroup", group);
    }

    function UpdateGroup(group) {
        return BaseAPIService.post("/api/Group/UpdateGroup", group);
    }
});