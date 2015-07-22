
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({

        GetVariationReport: GetVariationReport,
        GetVolumeReportData: GetVolumeReportData,
        GetTrafficVolumes:GetTrafficVolumes,
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


        console.log("asdsad");



        //Response.Clear();
        //Response.Buffer = true;
        //Response.Charset = "";
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Response.ContentType = "application/vnd.ms-excel";
        //Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName); //File Name
        //Response.WriteFile(filePath) //Save excel file path on server
        //Response.Flush();
        //Response.End();


        //$.ajax({
        //    dataType: "json",
        //    type: "POST",
        //    url: "/api/BillingStatistics/Export",
        //    success: function(response) {
        //        if (response && response.Uri && response.Uri.length) {
        //            window.location.href = response.Uri;
        //        }
        //    }
        //});



        return "/api/BillingStatistics/Export";
        //        data: data;

        //$('#hidden-excel-form').bind("submit", successCallback)
        //$('#hidden-excel-form').submit()



        //if( $('#hidden-excel-form').length < 1){

            //$('<form>').attr(method= 'POST', id= 'hidden-excel-form', action= '/api/BillingStatistics/Export').appendTo('body');

            //$('#hidden-excel-form').bind("submit", successCallback)
            //$('#hidden-excel-form').submit()
        //}
    }

    function GetVolumeReportData(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod, selectedTrafficReport) {
        return BaseAPIService.get("/api/BillingStatistics/GetVolumeReportData", {
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

    function GetTrafficVolumes(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod) {
        return BaseAPIService.get("/api/BillingStatistics/GetTrafficVolumes", {
            fromDate: fromDate,
            toDate: toDate,
            selectedCustomers: selectedCustomers,
            selectedSuppliers: selectedSuppliers,
            selectedZones: selectedZones,
            attempts: attempts,
            selectedTimePeriod: selectedTimePeriod
        });
    }
});