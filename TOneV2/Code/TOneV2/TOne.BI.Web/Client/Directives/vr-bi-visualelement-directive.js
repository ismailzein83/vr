'use strict';


app.directive('vrBiVisualelement', ['$compile', 'BIAPIService', 'BIUtilitiesService', function ($compile, BIAPIService, BIUtilitiesService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            timedimensiontype: '=',
            fromdate: '=',
            todate: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biVisualElement = new BIVisualElement(ctrl, ctrl.settings, retrieveDataOnLoad, BIAPIService);
            biVisualElement.initializeController();

            biVisualElement.defineAPI();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    var type = $scope.$parent.$eval(iAttrs.type);
                    switch(type)
                    {
                        case "chart": iElem.html('<vr-chart on-ready="ctrl.onChartReady" menuactions="ctrl.chartMenuActions"></vr-chart>');
                            break;
                        case "datagrid":
                            iElem.html('<vr-datagrid datasource="ctrl.data" on-ready="ctrl.onGridReady" maxheight="300px">'
                                        + '<vr-datagridcolumn ng-show="ctrl.showEntityColumn" headertext="ctrl.entityType.description" field="\'EntityName\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-show="ctrl.showTimeColumn" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>'
                                        +'<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType.description" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>'
                                    +'</vr-datagrid>');
                            break;
                    }
                        
                    $compile(iElem.contents())($scope);
                }
            }
        }

    };

    function BIVisualElement(ctrl, settings, retrieveDataOnLoad, BIAPIService) {

        var chartAPI;
        var gridAPI;

        function initializeController() {
            ctrl.onChartReady = function (api) {
                chartAPI = api;
                if (retrieveDataOnLoad)
                    retrieveData();
            };

            ctrl.onGridReady = function (api) {
                gridAPI = api;
                if (retrieveDataOnLoad)
                    retrieveData();
            }

            ctrl.entityType = settings.entityType;
            ctrl.measureTypes = settings.measureTypes;
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {
            var measureTypeValues = [];
            angular.forEach(settings.measureTypes, function (measureType) {
                measureTypeValues.push(measureType.value);
            });
            switch (settings.operationType) {
                case "TopEntities":
                    ctrl.showEntityColumn = true;
                    return BIAPIService.GetTopEntities(settings.entityType.value, measureTypeValues[0], ctrl.fromdate, ctrl.todate, 10, measureTypeValues)
                        .then(function (response) {
                            if (chartAPI != undefined)
                                refreshPIEChart(response);
                            if (gridAPI != undefined)
                                refreshDataGrid(response);
                        });
                case "MeasuresGroupedByTime":
                    ctrl.showTimeColumn = true;
                    return BIAPIService.GetMeasureValues(ctrl.timedimensiontype.value, ctrl.fromdate, ctrl.todate, measureTypeValues)
                        .then(function (response) {
                            var dontFillGroup = chartAPI != undefined ? false : true;
                            BIUtilitiesService.fillDateTimeProperties(response, ctrl.timedimensiontype.value, ctrl.fromdate, ctrl.todate, dontFillGroup);
                            if (chartAPI != undefined)
                                refreshChart(response);
                            if (gridAPI != undefined)
                                refreshDataGrid(response);
                        });
                    break;
            }
        }

        function refreshDataGrid(response) {
            ctrl.data = response;
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

