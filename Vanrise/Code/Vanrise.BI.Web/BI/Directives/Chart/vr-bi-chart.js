﻿'use strict';


app.directive('vrBiChart', ['BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'BIConfigurationAPIService', 'VRModalService', 'UtilsService', 'VRNotificationService', function (BIAPIService, BIUtilitiesService, BIVisualElementService, BIConfigurationAPIService, VRModalService, UtilsService, VRNotificationService) {

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

            var biChart = new BIChart(ctrl, $scope);
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
        if (previewmode == undefined) {
            return '<vr-section title="{{ctrl.title}}"><div ng-if="ctrl.isAllowed==false"  ng-class="\'{{ctrl.class}}\'"  >  ' +
               '<div style="padding-top:115px;" > <div class="alert alert-danger ng-scope" role="alert" style=""> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission, Please Contact Your Administrator..!!</div> </div>' +
                '</div><div ng-if="!ctrl.isAllowed && ctrl.chart"> <img src="/Client/Images/chartpermission.jpg" width="100%"/></div><div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData"><vr-chart on-ready="ctrl.onChartReady" menuactions="ctrl.chartMenuActions"></vr-chart></div></vr-section>';
        }
        else
            return '<vr-section title="{{ctrl.title}}"></br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.EntityType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.MeasureTypes" vr-disabled="true"></vr-textbox></vr-section>'

    }
    function BIChart(ctrl, $scope) {
        var chartAPI;
        var measures = [];
        var entity = [];
        var directiveSettings = {};
       
        function initializeController() {

           
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;

            api.load=function (payload)
            {

                if (payload != undefined) {
                    ctrl.title = payload.title;
                    ctrl.settings = payload.settings;
                    ctrl.filter = payload.filter;
                }
                return UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities]).then(function () {
                    directiveSettings = {
                        EntityType: entity,
                        MeasureTypes: measures
                    }
                  
                    if (payload != undefined && !payload.previewMode) {

                        if (!BIUtilitiesService.checkPermissions(measures)) {
                            ctrl.isAllowed = false;
                            return;
                        }
                        ctrl.isAllowed = true;
                        ctrl.onChartReady = function (api) {
                            chartAPI = api;
                        
                            //chartAPI.onDataItemClicked = function (item) {
                            //    BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);
                            //};
                            //if (retrieveDataOnLoad)
                            //    retrieveData();
                            return retrieveData(ctrl.filter);
                        };
                        if(chartAPI !=undefined)
                            return retrieveData(ctrl.filter);
                    }
                    getClassType();
                });
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getClassType() {
            if (ctrl.settings.OperationType == "TopEntities" && ctrl.settings.IsPieChart)
                ctrl.class = "piechartpermission";
            else
                ctrl.class = "chartpermission";
        }

        function retrieveData(filter) {
           
            if (!ctrl.isAllowed)
                return;
            ctrl.isGettingData = true;
            return BIVisualElementService.retrieveWidgetData(ctrl, ctrl.settings, filter)

                .then(function (response) {
                    if (ctrl.settings.IsPieChart && ctrl.settings.OperationType == "TopEntities")
                        refreshPIEChart(response);
                    else {
                        BIUtilitiesService.fillDateTimeProperties(response, filter.timeDimensionType.value, filter.fromDate, filter.toDate, false);
                        refreshChart(response);
                    }
                }).finally(function () {
                   
                    ctrl.isGettingData = false;
                });

        }

        function refreshPIEChart(response) {
            var chartDefinition = {
                type: "pie",
                title: directiveSettings.EntityType[0].DisplayName,
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
                type: ctrl.settings.DefinitionType,
                yAxisTitle: "Value"
            };
            var xAxisDefinition;
            switch (ctrl.settings.OperationType) {
                case "TopEntities": xAxisDefinition = { titlePath: "EntityName" }; break;
                case "MeasuresGroupedByTime": xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" }; break;
            }
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
            measures.length = 0;
            return BIConfigurationAPIService.GetMeasures().then(function (response) {
                for (var i = 0; i < ctrl.settings.MeasureTypes.length; i++) {
                    var value = UtilsService.getItemByVal(response, ctrl.settings.MeasureTypes[i], 'Name');
                    if (value != null)
                        measures.push(value);
                }
            });
        }

        function loadEntities() {
            entity.length = 0;
            return BIConfigurationAPIService.GetEntities().then(function (response) {
                for (var i = 0; i < ctrl.settings.EntityType.length; i++)
                    entity.push(UtilsService.getItemByVal(response, ctrl.settings.EntityType[i], 'Name'));
            });
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }
    return directiveDefinitionObject;
}]);

