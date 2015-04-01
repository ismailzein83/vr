
app.service('AnalyticsAPIService', function (BaseAPIService) {

    return ({
        GetTopNDestinations: GetTopNDestinations,
        GetTrafficStatisticMeasureList: GetTrafficStatisticMeasureList,
        GetTrafficStatisticSummary: GetTrafficStatisticSummary
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

    function GetTrafficStatisticSummary(tempTableKey, groupKeys, from, to, fromRow, toRow, orderBy, isDescending) {
        return BaseAPIService.get("/api/Analytics/GetTrafficStatisticSummary",
           {
               tempTableKey: tempTableKey,
               groupKeys: groupKeys,
               from: from,
               to: to,
               fromRow: fromRow,
               toRow: toRow,
               orderBy: orderBy,
               isDescending: isDescending
           }
          );
    }

});