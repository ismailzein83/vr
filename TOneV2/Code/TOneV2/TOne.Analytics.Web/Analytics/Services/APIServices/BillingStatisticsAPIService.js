
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({

        GetVariationReport: GetVariationReport,
        GetVolumeReportData: GetVolumeReportData
    });

    function GetVariationReport(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow,entityType,entityID,groupingBy) {
        return BaseAPIService.get("/api/BillingStatistics/GetVariationReport",
            {
                selectedDate: selectedDate,
                periodCount: periodCount,
                timePeriod: timePeriod,
                variationReportOption: variationReportOption,
                fromRow: fromRow,
                toRow: toRow,
                entityType: entityType,
                entityID: entityID,
                groupingBy: groupingBy

            }
            );
    }

    function GetVolumeReportData(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod, selectedTrafficReport) {
        return BaseAPIService.get("/api/Volume/GetVolumeReportData", {
            fromDate: fromDate,
            toDate: toDate,
            selectedCustomers: selectedCustomers,
            selectedSuppliers: selectedSuppliers,
            selectedZones: selectedZones,
            attempts: attempts,
            selectedTimePeriod: selectedTimePeriod,
            selectedTrafficReport: selectedTrafficReport
        });
    }
});