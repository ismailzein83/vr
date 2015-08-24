(function(app) {

    "use strict";

    app.service('AnalyticsAPIService', ['BaseAPIService', function (baseApiService) {

        return ({
            GetTopNDestinations: GetTopNDestinations,
            GetTrafficStatisticMeasureList: GetTrafficStatisticMeasureList,
            GetTrafficStatisticSummary: GetTrafficStatisticSummary,
            GetTrafficStatistics: GetTrafficStatistics,
            getReleaseCodeStatistics: getReleaseCodeStatistics
        });

        function getReleaseCodeStatistics(input) {
            return baseApiService.post("/api/TrafficMonitor/GetReleaseCodeStatistics", input);
        }

        function GetTopNDestinations(fromDate, toDate, from, to, topCount, showSupplier, groupByCodeGroup, codeGroup) {
            return baseApiService.get("/api/TrafficMonitor/GetTopNDestinations",
                {
                    fromDate: fromDate,
                    toDate: toDate,
                    from: from,
                    to: 20,
                    topCount: 20,
                    showSupplier: showSupplier,
                    groupByCodeGroup: groupByCodeGroup,
                    codeGroup: codeGroup
                });
        }

        function GetTrafficStatisticMeasureList() {
            return baseApiService.get("/api/TrafficMonitor/GetTrafficStatisticMeasureList",{});
        }

        function GetTrafficStatisticSummary(getTrafficStatisticSummaryInput) {
            return baseApiService.post("/api/TrafficMonitor/GetTrafficStatisticSummary", getTrafficStatisticSummaryInput);
        }

        function GetTrafficStatistics(filterByColumn, columnFilterValue, from, to) {
            return baseApiService.get("/api/TrafficMonitor/GetTrafficStatistics",
               {
                   filterByColumn: filterByColumn,
                   columnFilterValue: columnFilterValue,
                   from: from,
                   to: to
               });
        }

    }]);

})(app);

