InOutTrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService', 'UtilsService', 'VRNotificationService', 'UtilsService'];
function InOutTrafficVolumeTemplateController($scope, BillingStatisticsAPIService, UtilsService, VRNotificationService, UtilsService) {

    var durationChartAPI;
    var amountChartAPI;
    var durationFlag = false;
    var amountFlag = false;
    defineScope();
    load();


    function defineScope() {

        $scope.durationChartReady = function (api) {
            durationChartAPI = api;
            if (durationFlag)
                updateDurationChart($scope.durationData, $scope.filter.timePeriod);
        };

        $scope.amountChartReady = function (api) {
            amountChartAPI = api;
            if (durationFlag)
                updateAmountChart($scope.durationData, $scope.filter.timePeriod);

        };

        $scope.subViewResultInOutTraffic.getData = function (filter) {
            $scope.filter = filter;
            return getDurations();
        }
    }
    function load() {
    }

    function getDurations() {
        var filter = $scope.filter;
        if (!filter.showChartsInPie) {
            return BillingStatisticsAPIService.CompareInOutTraffic(filter.fromDate, filter.toDate, filter.requiredCustomerId, filter.timePeriod, filter.showChartsInPie)
                .then(function (response) {
                    $scope.durationData = response;
                    if (durationChartAPI != undefined)
                        updateDurationChart($scope.durationData, filter.timePeriod);
                    if (amountChartAPI != undefined)
                        updateAmountChart($scope.durationData, filter.timePeriod);
                    durationFlag = true;
                });
        }

        else {
            return BillingStatisticsAPIService.ExportInOutTraffic(filter.fromDate, filter.toDate, filter.requiredCustomerId, filter.timePeriod)
                .then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
        }
    }

    function updateDurationChart(data, timePeriod) {


        var chartDefinition = {
            type: "column",
            title: "In/Out Traffic Volumes -Duration",
            yAxisTitle: "Duration"
        };
        var result = [];
        var seriesDefinitions = [];

        var xAxisDefinition = { titlePath: "xValue" };

        var dates = [];



        if (timePeriod == 0)
            dates = [""];
        else {
            for (var i = 0; i < data.length; i++)
                if (i == 0)
                    dates.push(data[i].Date);
                else {
                    var j = i - 1;
                    if (data[i].Date != data[j].Date)
                        dates.push(data[i].Date);
                }
        }


        angular.forEach(dates, function (itm) {

            result.push({
                xValue: itm,
                Values: []
            });
        });



        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];

            seriesDefinitions.push({
                title: dataItem.TrafficDirection,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < dates.length ; j++) {


                if (dataItem.Date == result[j].xValue)
                    result[j].Values[i] = dataItem.Duration;
                else result[j].Values[i] = 0;

            }
        }



        durationChartAPI.renderChart(result, chartDefinition, seriesDefinitions, xAxisDefinition);

    }
    function updateAmountChart(data, timePeriod) {
        var chartDefinition2 = {
            type: "column",
            title: "In/Out Traffic Volumes -Net Sale Amounts",
            yAxisTitle: "Amounts"
        };

        var result2 = [];
        var seriesDefinitions2 = [];

        var xAxisDefinition2 = { titlePath: "xValue" };
        var dates = [];

        if (timePeriod == 0)
            dates = [""];
        else {
            for (var i = 0; i < data.length; i++)
                if (i == 0)
                    dates.push(data[i].Date);
                else {
                    var j = i - 1;
                    if (data[i].Date != data[j].Date)
                        dates.push(data[i].Date);
                }
        }

        angular.forEach(dates, function (itm) {
            result2.push({
                xValue: itm,
                Values: []
            });
        });

        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions2.push({
                title: dataItem.TrafficDirection,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < dates.length ; j++) {

                if (dataItem.Date == result2[j].xValue)
                    result2[j].Values[i] = dataItem.Net;
                else result2[j].Values[i] = 0;
            }
        }

        amountChartAPI.renderChart(result2, chartDefinition2, seriesDefinitions2, xAxisDefinition2);
    }
};
appControllers.controller('Analytics_InOutTrafficVolumeTemplateController', InOutTrafficVolumeTemplateController);
