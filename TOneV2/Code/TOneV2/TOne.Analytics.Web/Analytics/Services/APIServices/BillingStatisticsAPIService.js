
app.service('BillingStatisticsAPIService', function (BaseAPIService, DataRetrievalResultTypeEnum) {
    return ({

        GetVariationReport: GetVariationReport,
        GetTrafficVolumes: GetTrafficVolumes,
        GetDestinationTrafficVolumes: GetDestinationTrafficVolumes,
        CompareInOutTraffic: CompareInOutTraffic,
        ExportCarrierProfile: ExportCarrierProfile,
        ExportInOutTraffic: ExportInOutTraffic
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

    function GetDestinationTrafficVolumes(fromDate, toDate, selectedCustomer, selectedSupplier, selectedZone, attempts, selectedTimePeriod, topDestination, isDuration) {
        return BaseAPIService.get("/api/BillingStatistics/GetDestinationTrafficVolumes", {
            fromDate: fromDate,
            toDate: toDate,
            customerId: selectedCustomer,
            supplierId: selectedSupplier,
            zoneId: selectedZone,
            attempts: attempts,
            timePeriod: selectedTimePeriod,
            topDestination: topDestination,
            isDuration: isDuration
        });
    }

    function CompareInOutTraffic(fromDate, toDate, customerId, timePeriod, showChartsInPie) {
        return BaseAPIService.get("/api/BillingStatistics/CompareInOutTraffic", {
            fromDate: fromDate,
            toDate: toDate,
            customerId: customerId,
            timePeriod: timePeriod,
            showChartsInPie: showChartsInPie
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

    function ExportInOutTraffic(fromDate, toDate, customerId,timePeriod) {
        return BaseAPIService.post("/api/BillingStatistics/ExportInOutTraffic",
            {
                FromDate: fromDate,
                ToDate: toDate,
                CustomerId: customerId,
                timePeriod: timePeriod
            },
            {
                responseTypeAsBufferArray: true,
                returnAllResponseParameters: true
            });
    }
});