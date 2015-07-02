'use strict';


app.directive('vrChartBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'BIConfigurationAPIService', 'VRModalService', 'UtilsService', 'VRNotificationService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);
           
            var biChart = new BIChart(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1,BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService);
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
        template: function (element, attrs) {          
            return getBIChartTemplate(attrs.previewmode);
        }

    };
    function getBIChartTemplate(previewmode) {
       // console.log(previewmode);
        if (previewmode!='true') {
            return '<vr-section title="{{ctrl.settings.EntityType}}/{{ctrl.settings.MeasureTypes}}"><vr-chart on-ready="ctrl.onChartReady" menuactions="ctrl.chartMenuActions"></vr-chart></vr-section>';
        }
        else
            return '</br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.EntityType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.MeasureTypes" vr-disabled="true"></vr-textbox>'
          


    }
    function BIChart(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService) {

        var chartAPI;
        var Measures = [];
        var Entities = [];
        var directiveSettings = {};
        loadSettings();
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
                title: directiveSettings.EntityType.DisplayName,
                yAxisTitle: "Value"
            };

            var seriesDefinitions = [{
                title: directiveSettings.MeasureTypes[0].DisplayName,
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
        function loadSettings() {
            UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities])
             .then(function () {
                 var MeasureTypes = [];
                 for (var i = 0; i < settings.MeasureTypes.length; i++) {
                     var value = UtilsService.getItemByVal(Measures, settings.MeasureTypes[i], 'Name');
                     if (value != null)
                         MeasureTypes.push(value);
                 }
                 directiveSettings = {
                     EntityType: UtilsService.getItemByVal(Entities, settings.EntityType, 'Name'),
                     MeasureTypes: MeasureTypes
                 }
             })
             .finally(function () {
             }).catch(function (error) {
             });
           
        }
        function loadMeasures() {
            return BIConfigurationAPIService.GetMeasures().then(function (response) {
                angular.forEach(response, function (itm) {
                    Measures.push(itm);
                });
            });
        }
        function loadEntities() {
            return BIConfigurationAPIService.GetEntities().then(function (response) {
                angular.forEach(response, function (itm) {
                    Entities.push(itm);
                });
            });
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }
    return directiveDefinitionObject;
}]);

