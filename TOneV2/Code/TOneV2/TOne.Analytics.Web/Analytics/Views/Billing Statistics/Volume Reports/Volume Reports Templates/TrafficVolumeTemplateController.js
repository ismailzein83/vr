TrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService'];
function TrafficVolumeTemplateController($scope, BillingStatisticsAPIService) {
       
    var chartAPI;
    defineScope();
    load();


    function defineScope() {
       
        $scope.chartReady = function (api) {
            chartAPI = api;
        };

        $scope.subViewResultTrafficVolumes.getData = function (filter) {
            return BillingStatisticsAPIService.GetTrafficVolumes(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod)
                .then(function (response) {                    
                    updateChart(response);
                });
        }
    }

    function load() {
      
    }

    function updateChart(data) {        

        var chartDefinition = {
            type: "column",
            title: "Traffic volumes"
        };
        var xAxisDefinition = {
            titlePath: "Date",
        };
        
        var seriesDefinitions = [{
            title: "Attempts",
            valuePath: "Attempts",
            type: "column"
        }, {
            title: "Durations",
            valuePath: "Duration",
            type: "column"
        }];
        chartAPI.renderChart(data, chartDefinition, seriesDefinitions, xAxisDefinition);

    }


};
appControllers.controller('Analytics_TrafficVolumeTemplateController', TrafficVolumeTemplateController);
