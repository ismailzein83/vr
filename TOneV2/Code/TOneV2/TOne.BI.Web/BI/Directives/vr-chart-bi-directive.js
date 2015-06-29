'use strict';


app.directive('vrChartBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biChart = new BIChart(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, VRModalService);
            biChart.initializeController();

            biChart.defineAPI();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function () {
            return '<vr-chart on-ready="ctrl.onChartReady" menuactions="ctrl.chartMenuActions"></vr-chart>';
        }

    };

    function BIChart(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, VRModalService) {

        var chartAPI;

        function initializeController() {
            ctrl.onChartReady = function (api) {
                chartAPI = api;
                chartAPI.onDataItemClicked = function (item) {
                    BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);
                };
                if (retrieveDataOnLoad)
                    retrieveData();
            };
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {

            return BIVisualElementService1.retrieveData1(ctrl, settings)
                .then(function (response) {
                           // console.log(response);
                    if (ctrl.isDateTimeGroupedData) {

                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.filter.timeDimensionType.value, ctrl.filter.fromDate, ctrl.filter.toDate, false);
                                refreshChart(response);
                            }
                            else
                                refreshPIEChart(response);
                        });
        }

        function refreshPIEChart(response) {

            var chartDefinition = {
                type: "pie",
                title: settings.EntityType,
                yAxisTitle: "Value"
            };

            var seriesDefinitions = [{
                title: settings.MeasureTypes[0],
                titlePath: "EntityName",
                valuePath: "Values[0]"
            }];

            chartAPI.renderSingleDimensionChart(response, chartDefinition, seriesDefinitions);
        }

        function refreshChart(response) {

            var chartDefinition = {
                type: "column",
                yAxisTitle: "Value"
            };
            var xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };

            var seriesDefinitions = [];
            for (var i = 0; i < settings.MeasureTypes.length; i++) {
                var measureType = settings.MeasureTypes[i];
                seriesDefinitions.push({
                    title: measureType,
                    valuePath: "Values[" + i + "]"
                });
            }

            chartAPI.renderChart(response, chartDefinition, seriesDefinitions, xAxisDefinition);
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }
    return directiveDefinitionObject;
}]);

