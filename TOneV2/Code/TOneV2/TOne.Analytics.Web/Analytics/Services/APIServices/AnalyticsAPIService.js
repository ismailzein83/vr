
app.service('AnalyticsAPIService', function (BaseAPIService) {

    return ({
        GetTopNDestinations: GetTopNDestinations,
        GetTrafficStatisticMeasureList: GetTrafficStatisticMeasureList,
        GetTrafficStatisticSummary: GetTrafficStatisticSummary,
        GetTrafficStatistics: GetTrafficStatistics,
    });

    function GetTopNDestinations(fromDate, toDate, from, to, topCount, showSupplier, groupByCodeGroup, codeGroup) {
        return BaseAPIService.get("/api/TrafficMonitor/GetTopNDestinations",
            {
                fromDate: fromDate,
                toDate: toDate,
                from: from,
                to: 20,
                topCount: 20,
                showSupplier: showSupplier,
                groupByCodeGroup: groupByCodeGroup,
                codeGroup: codeGroup
            }
           );
    }

    function GetTrafficStatisticMeasureList() {
        return BaseAPIService.get("/api/TrafficMonitor/GetTrafficStatisticMeasureList",
            {
            }
           );
    }

    function GetTrafficStatisticSummary(getTrafficStatisticSummaryInput) {
        return BaseAPIService.post("/api/TrafficMonitor/GetTrafficStatisticSummary", getTrafficStatisticSummaryInput);
    }

    function GetTrafficStatistics(filterByColumn, columnFilterValue, from, to) {
        return BaseAPIService.get("/api/TrafficMonitor/GetTrafficStatistics",
           {
               filterByColumn: filterByColumn,
               columnFilterValue: columnFilterValue,
               from: from,
               to: to
           }
          );
    }

});