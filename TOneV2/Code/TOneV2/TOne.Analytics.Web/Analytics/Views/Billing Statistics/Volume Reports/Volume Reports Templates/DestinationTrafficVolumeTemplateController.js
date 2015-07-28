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
            return BillingStatisticsAPIService.GetDestinationTrafficVolumes(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod, filter.topDestination)
                .then(function (response) {
                    updateDurationChart(response,filter.topDestination);
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

    function updateDurationChart(data,top) {
        var chartDefinition = {
                    type: "column",
                    title: "Destination Traffic Volumes -Duration"
        };
        var result = [];
        var seriesDefinitions = [];
        for (var i = 0; i < top; i++) {
            result.push({ ZoneName: data.TopZones[i].ZoneName, DurationValue: data.ValuesPerDate.Values, Time: data.ValuesPerDate.Time[i] });
            var dataItem = result[i];
         
            seriesDefinitions.push({
                title: dataItem.ZoneName,
                valuePath: "DurationValue["+i+"]",
                type: "column" //areaspline 
            });
      
        }
               
        console.log(result);
        var xAxisDefinition = { titlePath: "Time" };
      

 
       
        console.log(seriesDefinitions);
 
        durationChartAPI.renderChart(result, chartDefinition, seriesDefinitions, xAxisDefinition);

    }


};
appControllers.controller('Analytics_DestinationTrafficVolumeTemplateController', DestinationTrafficVolumeTemplateController);
