app.service('VolumeReportsAPIService', function (BaseAPIService) {

    return ({
        GetVolumeReportData: GetVolumeReportData
    });

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