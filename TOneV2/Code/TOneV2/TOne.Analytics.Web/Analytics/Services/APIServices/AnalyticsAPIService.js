
app.service('AnalyticsAPIService', function (BaseAPIService) {

    return ({
        GetTopNDestinations: GetTopNDestinations,
        GetTrafficStatisticMeasureList: GetTrafficStatisticMeasureList,
        GetTrafficStatisticSummary: GetTrafficStatisticSummary,
        GetTrafficStatistics: GetTrafficStatistics,
        ExportTrafficStatisticSummary: ExportTrafficStatisticSummary
    });

    function GetTopNDestinations(fromDate, toDate, from, to, topCount, showSupplier, groupByCodeGroup, codeGroup) {
        return BaseAPIService.get("/api/Analytics/GetTopNDestinations",
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
        return BaseAPIService.get("/api/Analytics/GetTrafficStatisticMeasureList",
            {
            }
           );
    }

    function GetTrafficStatisticSummary(getTrafficStatisticSummaryInput) {
        return BaseAPIService.post("/api/Analytics/GetTrafficStatisticSummary", getTrafficStatisticSummaryInput);
    }
    function ExportTrafficStatisticSummary(tempTableKey) {
        return BaseAPIService.get("/api/Analytics/ExportTrafficStatisticSummary", 
            {tempTableKey:tempTableKey
    }, {
        returnAllResponseParameters: true,
        responseTypeAsBufferArray: true
    });
    }

    function GetTrafficStatistics(filterByColumn, columnFilterValue, from, to) {
        return BaseAPIService.get("/api/Analytics/GetTrafficStatistics",
           {
               filterByColumn: filterByColumn,
               columnFilterValue: columnFilterValue,
               from: from,
               to: to
           }
          );
    }

});