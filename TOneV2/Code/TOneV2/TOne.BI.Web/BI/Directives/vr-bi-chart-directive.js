'use strict';


app.directive('vrBiChart', ['BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'VRModalService', function (BIAPIService, BIUtilitiesService, BIVisualElementService, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            timedimensiontype: '=',
            fromdate: '=',
            todate: '=',
           
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biChart = new BIChart(ctrl, ctrl.settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, VRModalService);
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

    function BIChart(ctrl, settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, VRModalService) {

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
            return BIVisualElementService.retrieveData(ctrl, settings)
                        .then(function (response) {
                            if (ctrl.isDateTimeGroupedData) {
                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.timedimensiontype.value, ctrl.fromdate, ctrl.todate, false);
                                refreshChart(response);
                            }
                            else
                                refreshPIEChart(response);
                        });
        }

        function refreshPIEChart(response) {

            var chartDefinition = {
                type: "pie",
                title: settings.entityType.description,
                yAxisTitle: "Value"
            };

            var seriesDefinitions = [{
                title: settings.measureTypes[0].description,
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
            for (var i = 0; i < settings.measureTypes.length; i++) {
                var measureType = settings.measureTypes[i];
                seriesDefinitions.push({
                    title: measureType.description,
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

