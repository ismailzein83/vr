BillingReportsController.$inject = ['$scope', 'ReportAPIService', 'CarrierAccountAPIService', 'ZonesService', 'BillingStatisticsAPIService', 'MainService', 'BaseAPIService'];

function BillingReportsController($scope, ReportAPIService, CarrierAccountAPIService, ZonesService, BillingStatisticsAPIService, MainService, BaseAPIService) {

    defineScope();
    load();

    $scope.export = function () {
        //var paramsurl = "?";
        //paramsurl += "fromDate=" + $scope.dateToString($scope.params.fromDate);
        //paramsurl += "&toDate=" + $scope.dateToString($scope.params.toDate);

        //window.open(MainService.getBaseURL() + BillingStatisticsAPIService.Export() + paramsurl, "_self");
        console.log($scope.params.top);

        return BaseAPIService.get("/api/BillingStatistics/Export", {
            fromDate: $scope.dateToString($scope.params.fromDate),
            toDate: $scope.dateToString($scope.params.toDate),
            customerId: $scope.params.customer.CarrierAccountID,
            topDestination: $scope.params.top
        },
            {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            }
            ).then(function (response) {
                var data = response.data;
                var status = response.status;
                var headers = response.headers;
                var octetStreamMime = 'application/octet-stream';
                var success = false;

                // Get the headers
                headers = headers();

                //// Get the filename from the x-filename header or default to "download.bin"
                //console.log(headers['content-disposition']);
                //var matcher = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/i;

                //var results = headers['content-disposition'].match(matcher);
                var filename = "myfile44.xls";

                // Determine the content type from the header or default to "application/octet-stream"
                var contentType = headers['content-type'] || octetStreamMime;
                console.log(contentType);
                //contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;";
                try {
                    // Try using msSaveBlob if supported
                    console.log("Trying saveBlob method ...");
                    var blob = new Blob([data], { type: contentType });
                    if (navigator.msSaveBlob)
                        navigator.msSaveBlob(blob, filename);
                    else {
                        // Try using other saveBlob implementations, if available
                        var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                        if (saveBlob === undefined) throw "Not supported";
                        saveBlob(blob, filename);
                    }
                    console.log("saveBlob succeeded");
                    success = true;
                } catch (ex) {
                    console.log("saveBlob method failed with the following exception:");
                    console.log(ex);
                }

                if (!success) {
                    // Get the blob url creator
                    var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                    if (urlCreator) {
                        // Try to use a download link
                        var link = document.createElement('a');
                        if ('download' in link) {
                            // Try to simulate a click
                            try {
                                // Prepare a blob URL
                                console.log("Trying download link method with simulated click ...");
                                var blob = new Blob([data], { type: contentType });
                                var url = urlCreator.createObjectURL(blob);
                                console.log(url);
                                link.setAttribute('href', url);

                                // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                link.setAttribute("download", filename);

                                // Simulate clicking the download link
                                var event = document.createEvent('MouseEvents');
                                event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                link.dispatchEvent(event);
                                console.log("Download link method with simulated click succeeded");
                                success = true;

                            } catch (ex) {
                                console.log("Download link method with simulated click failed with the following exception:");
                                console.log(ex);
                            }
                        }

                        if (!success) {
                            // Fallback to window.location method
                            try {
                                // Prepare a blob URL
                                // Use application/octet-stream when using window.location to force download
                                console.log("Trying download link method with window.location ...");
                                var blob = new Blob([data], { type: octetStreamMime });
                                var url = urlCreator.createObjectURL(blob);
                                window.location = url;
                                console.log("Download link method with window.location succeeded");
                                success = true;
                            } catch (ex) {
                                console.log("Download link method with window.location failed with the following exception:");
                                console.log(ex);
                            }
                        }

                    }
                }

                if (!success) {
                    // Fallback to window.open method
                    console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                    window.open(httpPath, '_blank', '');
                }
            });



    };

    $scope.reportsTypes = [];
    $scope.optionsCustomers = [];
    $scope.optionsSuppliers = [];
    $scope.optionsZone = [];
    $scope.reportsTypes = [];
    $scope.params = {
        fromDate: "",
        toDate: "",
        groupByCustomer: false,
        customer: null,
        supplier: null,
        zone: null,
        isCost: false,
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top: 10
    }
    function defineScope() {

        $scope.optionsZones = function (filterText) {
            return ZonesService.getSalesZones(filterText);
        };

        $scope.openReport = function () {
            var paramsurl = "";
            paramsurl += "reportId=" + $scope.reporttype.ReportDefinitionId;
            paramsurl += "&fromDate=" + $scope.dateToString($scope.params.fromDate);
            paramsurl += "&toDate=" + $scope.dateToString($scope.params.toDate);
            paramsurl += "&groupByCustomer=" + $scope.params.groupByCustomer;
            paramsurl += "&isCost=" + $scope.params.isCost;
            paramsurl += "&service=" + $scope.params.service;
            paramsurl += "&commission=" + $scope.params.commission;
            paramsurl += "&bySupplier=" + $scope.params.bySupplier;
            paramsurl += "&isExchange=" + $scope.params.isExchange;
            paramsurl += "&margin=" + $scope.params.margin;
            paramsurl += "&top=" + $scope.params.top;
            paramsurl += "&zone=" + (($scope.params.zone == null) ? 0 : $scope.params.zone.ZoneId);
            paramsurl += "&customer=" + (($scope.params.customer == null) ? "" : $scope.params.customer.CarrierAccountID);
            paramsurl += "&supplier=" + (($scope.params.supplier == null) ? "" : $scope.params.supplier.CarrierAccountID);

            if ($scope.reporttype.ReportDefinitionId != 22)
                window.open("/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
            else
                $scope.export();
        }
        $scope.resetReportParams = function () {

            $scope.params = {
                fromDate: "",
                toDate: "",
                groupByCustomer: false,
                customer: null,
                supplier: null,
                zone: null,
                isCost: false,
                service: false,
                commission: false,
                bySupplier: false,
                margin: 10,
                isExchange: false,
                top: 10
            }
        }
    }

    function load() {
        loadReportTypes();
        loadCustomers();
        loadSuppliers();
    }

    function loadReportTypes() {
        ReportAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        });
    }
    function loadCustomers() {
        CarrierAccountAPIService.GetCarriers(1).then(function (response) {
            $scope.optionsCustomers = response;
        });
    }
    function loadSuppliers() {
        CarrierAccountAPIService.GetCarriers(2).then(function (response) {
            $scope.optionsSuppliers = response;
        });
    }

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);