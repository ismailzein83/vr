app.service("SwitchTrunkAPIService", function (BaseAPIService) {

    return ({
        GetFilteredSwitchTrunks: GetFilteredSwitchTrunks
    });

    function GetFilteredSwitchTrunks(input) {
        return BaseAPIService.post("/api/SwitchTrunk/GetFilteredSwitchTrunks", input);
    }
});
