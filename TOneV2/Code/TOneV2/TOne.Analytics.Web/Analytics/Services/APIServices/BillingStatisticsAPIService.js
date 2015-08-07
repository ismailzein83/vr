
app.service('BillingStatisticsAPIService', function (BaseAPIService, DataRetrievalResultTypeEnum) {
    return ({

        GetVariationReport: GetVariationReport,
        GetTrafficVolumes: GetTrafficVolumes,
        GetDestinationTrafficVolumes: GetDestinationTrafficVolumes,
        ExportCarrierProfile: ExportCarrierProfile
    });

    function GetVariationReport(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow, entityType, entityID, groupingBy) {
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

    function GetDestinationTrafficVolumes(fromDate, toDate, selectedCustomer, selectedSupplier, selectedZone, attempts, selectedTimePeriod, topDestination) {
        return BaseAPIService.get("/api/BillingStatistics/GetDestinationTrafficVolumes", {
            fromDate: fromDate,
            toDate: toDate,
            customerId: selectedCustomer,
            supplierId: selectedSupplier,
            zoneId: selectedZone,
            attempts: attempts,
            timePeriod: selectedTimePeriod,
            topDestination: topDestination
        });
    }

    function ExportCarrierProfile(fromDate, toDate, topDestination, customerId) {
        return BaseAPIService.post("/api/BillingStatistics/ExportCarrierProfile",
            {
                FromDate: fromDate,
                ToDate: toDate,
                TopDestination: topDestination,
                CustomerId: customerId
            },
            {
                responseTypeAsBufferArray: true,
                returnAllResponseParameters: true
            });
    }
});