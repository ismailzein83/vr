'use strict';


app.directive('vrChartBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'BIConfigurationAPIService', 'VRModalService', 'UtilsService', 'VRNotificationService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            title: '=',
            filter: '=',
            previewmode: '@',
           
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);
           
            var biChart = new BIChart(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1,BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService);
            biChart.initializeController();

          

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
        if (previewmode!='true') {
            return '<vr-section title="{{ctrl.title}}"><div ng-if="!ctrl.isAllowed"  ng-class="\'{{ctrl.class}}\'"  >  ' +
               '<div style="padding-top:115px;" > <div class="alert alert-danger ng-scope" role="alert" style=""> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission To See This Widget..!!</div> </div>' +
                '</div><div ng-if="!ctrl.isAllowed && ctrl.chart"> <img src="/Client/Images/chartpermission.jpg" width="100%"/></div><div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData"><vr-chart on-ready="ctrl.onChartReady" menuactions="ctrl.chartMenuActions"></vr-chart></div></vr-section>';
        }
        else
            return '<vr-section title="{{ctrl.title}}"></br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.EntityType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.MeasureTypes" vr-disabled="true"></vr-textbox></vr-section>'
          
    }
    function BIChart(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService) {
        var chartAPI;
        var measures = [];
        var entity;
        var directiveSettings = {};
        getClassType();
        function getClassType() {
            switch(settings.OperationType)
            {
                case "TopEntities": ctrl.class = "piechartpermission"; break;
                case "MeasuresGroupedByTime": ctrl.class = "chartpermission"; break;
            }

           
        }
        function initializeController() {
            UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities])
            .then(function () {
                if (!BIUtilitiesService.checkPermissions(measures)) {
                    ctrl.isAllowed = false;
                    return;
                }

                directiveSettings = {
                    EntityType: entity,
                    MeasureTypes: measures
                }
                ctrl.isAllowed = true;
                ctrl.onChartReady = function (api) {
                    chartAPI = api;
                    chartAPI.onDataItemClicked = function (item) {
                        BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);
                    };
                    //if (retrieveDataOnLoad)
                    //    retrieveData();
                };
               defineAPI();

            })
            .finally(function () {
            }).catch(function (error) {
            });
           
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData(filter) {
            if (!ctrl.isAllowed)
                return;
            ctrl.isGettingData = true;
            return BIVisualElementService1.retrieveWidgetData(ctrl, settings,filter)

                .then(function (response) {
                           
                    if (ctrl.isDateTimeGroupedData) {
                                BIUtilitiesService.fillDateTimeProperties(response, filter.timeDimensionType.value, filter.fromDate, filter.toDate, false);
                                refreshChart(response);
                            }
                            else
                                refreshPIEChart(response);
                }).finally(function () {
                    ctrl.isGettingData = false;
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
                type: settings.DefinitionType,
                yAxisTitle: "Value"
            };
            var xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };

            var seriesDefinitions = [];
            for (var i = 0; i < directiveSettings.MeasureTypes.length; i++) {
                var measureType = directiveSettings.MeasureTypes[i];
                seriesDefinitions.push({
                    title: measureType.DisplayName,
                    valuePath: "Values[" + i + "]"
                });
            }

            chartAPI.renderChart(response, chartDefinition, seriesDefinitions, xAxisDefinition);
        }
       
        function loadMeasures() {
            return BIConfigurationAPIService.GetMeasures().then(function (response) {
                for (var i = 0; i < settings.MeasureTypes.length; i++) {
                    var value = UtilsService.getItemByVal(response, settings.MeasureTypes[i], 'Name');
                    if (value != null)
                        measures.push(value);
                }
            });
        }
        function loadEntities() {
            return BIConfigurationAPIService.GetEntities().then(function (response) {
                    entity = UtilsService.getItemByVal(response, settings.EntityType, 'Name');
            });
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }
    return directiveDefinitionObject;
}]);

