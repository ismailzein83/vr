(function (angular, app) {

    "use strict";

    function vrDirectiveObj(analyticsService, UtilsService, VRNotificationService, BusinessEntityAPIService, ZonesService, CarrierAccountConnectionAPIService, CarrierTypeEnum,
        GenericAnalyticDimensionEnum, CurrencyAPIService, GenericAnalyticAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                dimensions: "=",
                filters: "=",
                periods: "=",
                measures: "=",
                fromdate: "=",
                todate: "=",
                currency: "=",
                onReady: '='
            },
            controller: function ($attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var gridApi = {};
                var periods = [];
                
                ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].name;

                ctrl.gridReady = function(api) {
                    gridApi = api;
                };

                ctrl.dimensionFields = [];

                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(gridApi);

                gridApi.LoadGrid = function () {
                    if (ctrl.periods.length > 0 || ctrl.dimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].name;

                    var measureValues = [];

                    
                    measureValues.length = 0;
                    if (gridApi == undefined)
                        return;

                    var dimensionValues = [];
                    periods.length = 0;

                    ctrl.dimensions.forEach(function (group) {
                        dimensionValues.push(group.value);
                    });

                    if (ctrl.periods == undefined)
                        ctrl.periods = [];
                    else {
                        dimensionValues.push(ctrl.periods.value);
                        periods.push(ctrl.periods);

                        if (ctrl.periods == GenericAnalyticDimensionEnum.Hour) {
                            dimensionValues.push(GenericAnalyticDimensionEnum.Date.value);
                            periods.push(GenericAnalyticDimensionEnum.Date);
                        }
                    }

              
                    for (var i = 0, len = ctrl.measures.length; i < len; i++) {
                        measureValues.push(ctrl.measures[i].value);
                    }

                    var query = {
                        Filters: ctrl.filters,
                        DimensionFields: dimensionValues,
                        MeasureFields: measureValues,
                        FromTime: ctrl.fromdate,
                        ToTime: ctrl.todate,
                        Currency: ctrl.currency,
                        WithSummary: $attrs.withsummary != undefined
                    }
                    
                    ctrl.selectedMeasures = ctrl.measures;
                    ctrl.selectedfilters = ctrl.filters;
                    
                    return gridApi.retrieveData(query);


                }



                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                    .then(function (response) {
                        ctrl.dimensionFields.length = 0;

                        ctrl.dimensions.forEach(function (group) {
                            ctrl.dimensionFields.push(group);
                        });

                        periods.forEach(function (group) {
                            ctrl.dimensionFields.push(group);
                        });

                        //if (isSummary)
                        //    gridApi.setSummary(response.Summary);

                        onResponseReady(response);
                    });
                };


                ctrl.checkExpandablerow = function (groupKeys) {
                    return groupKeys.length !== ctrl.groupKeys.length;
                };


                angular.extend(this, {
                    //dataRetrievalFunction: dataRetrievalFunction,
                    //gridReady: gridReady
                });
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytics/Directives/vr-generalgrid.html";
            }
        };

        return directiveDefinitionObject;
    }

    vrDirectiveObj.$inject = ['AnalyticsService', 'UtilsService', 'VRNotificationService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CarrierAccountConnectionAPIService', 'CarrierTypeEnum', 'GenericAnalyticDimensionEnum', 'CurrencyAPIService', 'GenericAnalyticAPIService'];

    app.directive('vrAnGeneralgrid', vrDirectiveObj);

})(angular, app);