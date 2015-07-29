app.service('GroupAPIService', function (BaseAPIService) {

    return ({
        GetFilteredGroups: GetFilteredGroups,
        GetGroup: GetGroup,
        GetGroups: GetGroups,
        AddGroup: AddGroup,
        UpdateGroup: UpdateGroup
    });

    function GetFilteredGroups(input) {
        return BaseAPIService.post("/api/Group/GetFilteredGroups", input);
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