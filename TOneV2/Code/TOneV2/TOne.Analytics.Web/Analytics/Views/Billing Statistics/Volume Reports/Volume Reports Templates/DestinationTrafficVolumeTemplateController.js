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
                    updateDurationChart(response);
                });
        }
    }

    function load() {

    }

    function updateDurationChart(data) {

        var chartDefinition = {
            type: "column",
            title: "Destination Traffic Volumes -Duration"
        };

        var xAxisDefinition = {
            titlePath: "SaleZoneName",
        };

        var seriesDefinitions = [{
            title: "Duration",
            valuePath: "Values",
            type: "areaspline"
        }];

        durationChartAPI.renderChart(data, chartDefinition, seriesDefinitions, xAxisDefinition);

    }


};
appControllers.controller('Analytics_DestinationTrafficVolumeTemplateController', DestinationTrafficVolumeTemplateController);
