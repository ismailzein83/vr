app.service('OrgChartAPIService', function (BaseAPIService) {
    return ({
        GetOrgCharts: GetOrgCharts,
        GetFilteredOrgCharts: GetFilteredOrgCharts,
        GetOrgChartById: GetOrgChartById,
        AddOrgChart: AddOrgChart,
        UpdateOrgChart: UpdateOrgChart,
        DeleteOrgChart: DeleteOrgChart
    });

    function GetOrgCharts() {
        return BaseAPIService.get('/api/OrgChart/GetOrgCharts');
    }

    function GetFilteredOrgCharts(input) {
        return BaseAPIService.post("/api/OrgChart/GetFilteredOrgCharts", input);
    }

    function GetOrgChartById(orgChartId) {
        return BaseAPIService.get("/api/OrgChart/GetOrgChartById", { orgChartId: orgChartId });
    }

    function AddOrgChart(orgChart) {
        return BaseAPIService.post("/api/OrgChart/AddOrgChart", orgChart);
    }

    function UpdateOrgChart(orgChart) {
        return BaseAPIService.post('/api/OrgChart/UpdateOrgChart', orgChart);
    }

    function DeleteOrgChart(orgChartId) {
        return BaseAPIService.get('/api/OrgChart/DeleteOrgChart', { orgChartId: orgChartId });
    }
});