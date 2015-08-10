DestinationTrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService'];
function DestinationTrafficVolumeTemplateController($scope, BillingStatisticsAPIService) {

    var durationChartAPI;
    defineScope();
    load();
  


    function defineScope() {
        
        $scope.durationChartReady = function (api) {
            durationChartAPI = api;
        };


        $scope.subViewResultDestinationTrafficVolumes.getData = function (filter) {
            return BillingStatisticsAPIService.GetDestinationTrafficVolumes(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod, filter.topDestination).then(function (response) {
                updateDurationChart(response);
            }).finally(function () {
               // $scope.isLoading = false;
            });
        }
    }

    function load() {

    }

    //function updateDurationChart(data) {

    //    var chartDefinition = {
    //        type: "column",
    //        title: "Destination Traffic Volumes -Duration"
    //    };

    //    var xAxisDefinition = {
    //        titlePath: "SaleZoneName",
    //    };

    //    var seriesDefinitions = [{
    //        title: "Duration",
    //        valuePath: "Value",
    //        type: "areaspline"
    //    }];

    //    durationChartAPI.renderChart(data, chartDefinition, seriesDefinitions, xAxisDefinition);

    //}

    function updateDurationChart(data) {
        console.log(data.length);
        var chartDefinition = {
                    type: "column",
                    title: "Destination Traffic Volumes -Duration",
                    yAxisTitle: "Destination Traffic Volumes -Duration"
        };
        var result = [];
        var seriesDefinitions = [];

        var xAxisDefinition = { titlePath: "xValue" };
        var times = [];

        //angular.forEach(data, function (itm) { times.push(itm.Time) } )
        for (var i = 0; i <= data.length;i++)
        times.push(data[0].Time[i]);

        console.log(times);

        angular.forEach(times, function (itm) {
            result.push({
                xValue: itm,
                Values: []
            });
        });
        console.log(result);

        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
         //   result.push({ ZoneName: dataItem.ZoneName, DurationValue: dataItem.Values, Time: dataItem.Time });
          //  console.log(dataItem);
            seriesDefinitions.push({
                title: dataItem.ZoneName,
                valuePath: "Values[" + i + "]", //dataItem.Values,
                type: "column" //areaspline 
            });
            for (var j = 0; j < times.length ; j++) {
                result[j].Values[i] = dataItem.Values[j];
            }
        }
      
        console.log(result);
       
        console.log(seriesDefinitions);
        durationChartAPI.renderChart(result, chartDefinition, seriesDefinitions, xAxisDefinition);

    }


};
appControllers.controller('Analytics_DestinationTrafficVolumeTemplateController', DestinationTrafficVolumeTemplateController);
