DestinationTrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService', 'UtilsService', 'VRNotificationService'];
function DestinationTrafficVolumeTemplateController($scope, BillingStatisticsAPIService, UtilsService, VRNotificationService) {

    var durationChartAPI;
    var attemptsChartAPI;
    var durationFlag=false;
    var attemptsFlag=false;
    defineScope();
    load();


    function defineScope() {

        $scope.durationChartReady = function (api) {
            durationChartAPI = api;
            if(durationFlag)
               updateDurationChart($scope.durationData);
        };

        $scope.attemptsChartReady = function (api) {
            attemptsChartAPI = api;
            if(attemptsFlag)
                updateAttemptsChart($scope.attemptsData);
        };

        $scope.subViewResultDestinationTrafficVolumes.getData = function (filter) {
            $scope.filter = filter;
           return UtilsService.waitMultipleAsyncOperations([getDurations, getAttempts])
           .catch(function (error) {
             VRNotificationService.notifyExceptionWithClose(error, $scope);
         });
        }
    }
    function load() {
    }
    function getDurations() {
        var filter = $scope.filter;
      return  BillingStatisticsAPIService.GetDestinationTrafficVolumes(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod, filter.topDestination, true)
           .then(function (response) {
               $scope.durationData = response;
               if (durationChartAPI!=undefined)
                   updateDurationChart($scope.durationData);
               durationFlag = true;
           });
    }
    function getAttempts() {
        var filter = $scope.filter;
        return BillingStatisticsAPIService.GetDestinationTrafficVolumes(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod, filter.topDestination, false)
               .then(function (response) {
                   $scope.attemptsData = response;
                   if (attemptsChartAPI != undefined)
                       updateAttemptsChart($scope.attemptsData);
                   attemptsFlag = true;
               });
    }
    function updateDurationChart(data) {
        var chartDefinition = {
            type: "column",
            title: "Destination Traffic Volumes -Duration",
            yAxisTitle: "Duration"
        };
        var result = [];
        var seriesDefinitions = [];

        var xAxisDefinition = { titlePath: "xValue" };
        var times = [];
        var period = data[0].Time.length;
        for (var i = 0; i < period; i++)
            times.push(data[0].Time[i]);
        angular.forEach(times, function (itm) {
            result.push({
                xValue: itm,
                Values: []
            });
        });
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions.push({
                title: dataItem.ZoneName,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < times.length ; j++) {
                result[j].Values[i] = dataItem.Values[j];
            }
        }
        durationChartAPI.renderChart(result, chartDefinition, seriesDefinitions, xAxisDefinition);

    }
    function updateAttemptsChart(data) {
        var chartDefinition2 = {
            type: "column",
            title: "Destination Traffic Volumes -Attempts",
            yAxisTitle: "Attempts"
        };
        var result2 = [];
        var seriesDefinitions2 = [];

        var xAxisDefinition2 = { titlePath: "xValue" };
        var times = [];

        var period = data[0].Time.length;
        for (var i = 0; i < period; i++)
            times.push(data[0].Time[i]);
        angular.forEach(times, function (itm) {
            result2.push({
                xValue: itm,
                Values: []
            });
        });
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions2.push({
                title: dataItem.ZoneName,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < times.length ; j++) {
                result2[j].Values[i] = dataItem.Values[j];
            }
        }
        attemptsChartAPI.renderChart(result2, chartDefinition2, seriesDefinitions2, xAxisDefinition2);
    }


};
appControllers.controller('Analytics_DestinationTrafficVolumeTemplateController', DestinationTrafficVolumeTemplateController);
