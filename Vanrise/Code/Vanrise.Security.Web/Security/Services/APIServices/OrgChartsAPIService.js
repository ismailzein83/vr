app.service('UsersAPIService', function (BaseAPIService) {
    return ({
        GetFilteredOrgCharts: GetFilteredOrgCharts
    });

    function GetFilteredOrgCharts(fromRow, toRow, name) {
        return BaseAPIService.get
            (
                "/api/OrgCharts/GetFilteredOrgCharts",
                {
                    fromRow: fromRow,
                    toRow: toRow,
                    name: name
                }
            );
    }
});