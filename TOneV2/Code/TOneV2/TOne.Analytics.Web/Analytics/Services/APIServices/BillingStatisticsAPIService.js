
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({

        GetVariationReport: GetVariationReport,
        GetTrafficVolumes: GetTrafficVolumes,
        GetDestinationTrafficVolumes : GetDestinationTrafficVolumes,
        Export: Export
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

    function Export() {
        return "/api/BillingStatistics/Export";
    }
    
    function GetTrafficVolumes(fromDate, toDate, selectedCustomer, selectedSupplier, selectedZone, attempts, selectedTimePeriod) {
        return BaseAPIService.get("/api/BillingStatistics/GetTrafficVolumes", {
            fromDate: fromDate,
            toDate: toDate,
            customerId: selectedCustomer,
            supplierId: selectedSupplier,
            zoneId: selectedZone,
            attempts: attempts,
            timePeriod: selectedTimePeriod
        });
    }

    function GetDestinationTrafficVolumes(fromDate, toDate, selectedCustomer, selectedSupplier, selectedZone, attempts, selectedTimePeriod,topDestination) {
        return BaseAPIService.get("/api/BillingStatistics/GetDestinationTrafficVolumes", {
            fromDate: fromDate,
            toDate: toDate,
            customerId: selectedCustomer,
            supplierId: selectedSupplier,
            zoneId: selectedZone,
            attempts: attempts,
            timePeriod: selectedTimePeriod,
            topDestination : topDestination
        });
    }
});