"use strict";

app.directive("vrWhsAnalyticsGenericgrid", ['UtilsService', 'VRNotificationService', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticAPIService', 'WhS_Analytics_GenericAnalyticMeasureEnum', 'VRUIUtilsService', 'WhS_Analytics_GenericAnalyticService',
    function (UtilsService, VRNotificationService,
        WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticAPIService, WhS_Analytics_GenericAnalyticMeasureEnum, VRUIUtilsService, WhS_Analytics_GenericAnalyticService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var genericGrid = new GenericGrid($scope, ctrl, $attrs);

                genericGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Analytics/Directives/Generic/Templates/GenericGridTemplate.html"

        };

        function GenericGrid($scope, ctrl, $attrs)
        {

            this.initializeController = initializeController;

            ctrl.selectedMeasures = [];
            ctrl.selectedDimensions = [];
            ctrl.datasource = [];
            ctrl.selectedfilters = [];
            ctrl.selectedPeriods = [];
            ctrl.fromTime;
            ctrl.toTime;
            ctrl.dimensions = [];
            ctrl.dimensionFields = [];
            ctrl.currentDimention = [];
            var gridApi;
            var measureValues = [];
            var dimensionValues = [];
            var gridDrillDownTabsObj;
            var isSummary = false;
            var parameters;

            function initializeController() {

                ctrl.getColor = function (dataItem, coldef)
                {
                    if (parameters != undefined)
                        return WhS_Analytics_GenericAnalyticService.getMeasureColor(dataItem, coldef, parameters);
                }

                ctrl.gridReady = function (api)
                {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI()
                    {
                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (query)
                        {
                            dimensionValues.length = 0;
                            ctrl.selectedDimensions.length = 0;
                            ctrl.selectedPeriods.length = 0;
                            ctrl.selectedMeasures.length = 0;
                            ctrl.dimensions.length = 0;
                            ctrl.dimensionFields.length = 0;
                            ctrl.currentDimention.length = 0;

                            var queryFinalized = loadGridQuery(query);

                            var drillDownDefinitions = [];

                            applyGroupKeysRules(ctrl.selectedDimensions, ctrl.dimensions);

                            for (var i = 0; i < ctrl.dimensions.length; i++)
                            {
                                var selectedDimensions = [];
                                var obj = ctrl.dimensions[i];
                                for (var j = 0; j < ctrl.selectedDimensions.length; j++)
                                    if (ctrl.selectedDimensions[j].value != obj.value)
                                        selectedDimensions.push(ctrl.selectedDimensions[j].value);
                                setDrillDownData(ctrl.dimensions[i], selectedDimensions)

                            }



                            function setDrillDownData(dimention, selectedDimensions)
                            {
                                var objData = {};
                                objData.title = dimention.name;
                                objData.directive = "vr-whs-analytics-genericgrid";
                                objData.loadDirective = function (directiveAPI, dataItem)
                                {
                                    for (var j = 0; j < ctrl.currentDimention.length; j++)
                                    {
                                        ctrl.selectedfilters.push(
                                        {
                                            Dimension: ctrl.currentDimention[j].value,
                                            FilterValues: [dataItem.DimensionValues[j].Id]
                                        });
                                    }

                                    dataItem.gridAPI = directiveAPI;

                                    var query = {
                                        Filters: ctrl.selectedfilters,
                                        DimensionFields: [dimention.value],
                                        MeasureFields: measureValues,
                                        DimensionsSelected: selectedDimensions,
                                        MeasureThreshold: undefined,
                                        FromTime: ctrl.fromTime,
                                        ToTime: ctrl.toTime,
                                        Currency: undefined,
                                        WithSummary: isSummary
                                    }
                                    return dataItem.gridAPI.loadGrid(query);
                                };
                                drillDownDefinitions.push(objData);
                            }

                            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, ctrl.gridMenuActions);
                            return gridApi.retrieveData(queryFinalized);

                        }

                        return directiveAPI;
                    }


                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
                {
                    ctrl.showGrid = true;
                    return WhS_Analytics_GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined)
                            {
                                for (var i = 0; i < response.Data.length; i++)
                                {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
                            if (isSummary)
                                gridApi.setSummary(response.Summary);

                            onResponseReady(response);
                        });
                };

                ctrl.checkExpandablerow = function (groupKeys) {
                    return groupKeys.length !== ctrl.groupKeys.length;
                };

                function loadGridQuery(query)
                {

                    ctrl.fromTime = query.FromTime;
                    ctrl.toTime = query.ToTime;

                    if (query.MeasureThreshold != undefined)
                    {
                        parameters = {
                            asr: query.MeasureThreshold.asr,
                            attempts: query.MeasureThreshold.attempts,
                            acd: query.MeasureThreshold.acd,
                        };
                    }

                    for (var p in WhS_Analytics_GenericAnalyticDimensionEnum)
                    {

                        ctrl.dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                        if (query.DimensionsSelected != undefined) {
                            for (var i = 0; i < query.DimensionsSelected.length; i++)
                            {
                                if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == query.DimensionsSelected[i])
                                {
                                    ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);

                                }

                            }
                        }
                        if (query.DimensionFields != undefined)
                        {
                            for (var i = 0; i < query.DimensionFields.length; i++)
                            {
                                if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == query.DimensionFields[i])
                                {
                                    ctrl.dimensionFields.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                    ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                    ctrl.currentDimention.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                }

                            }
                            dimensionValues = query.DimensionFields;
                        }
                        if (query.FixedDimensionFields != undefined)
                        {
                            if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == query.FixedDimensionFields)
                            {
                                ctrl.dimensionFields.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                ctrl.currentDimention.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                ctrl.selectedPeriods.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                                dimensionValues.push(query.FixedDimensionFields);
                                if (query.FixedDimensionFields == WhS_Analytics_GenericAnalyticDimensionEnum.Hour.value)
                                {
                                    ctrl.dimensionFields.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                    dimensionValues.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date.value);
                                    ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                    ctrl.currentDimention.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                    ctrl.selectedPeriods.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                }
                            }

                        }
                    }

                    if (query.MeasureFields != undefined)
                    {
                        for (var i = 0; i < query.MeasureFields.length; i++)
                            for (var p in WhS_Analytics_GenericAnalyticMeasureEnum)
                                if (WhS_Analytics_GenericAnalyticMeasureEnum[p].value == query.MeasureFields[i])
                                    ctrl.selectedMeasures.push(WhS_Analytics_GenericAnalyticMeasureEnum[p]);
                        measureValues = query.MeasureFields;
                    }
                    if (query.Filters != undefined)
                        ctrl.selectedfilters = query.Filters;
                    isSummary = $attrs.withsummary != undefined;
                    var queryFinalized = {
                        Filters: query.Filters,
                        DimensionFields: dimensionValues,
                        MeasureFields: query.MeasureFields,
                        FromTime: query.FromTime,
                        ToTime: query.ToTime,
                        Currency: query.Currency,
                        WithSummary: isSummary
                    }

                    if (ctrl.selectedPeriods.length > 0 || ctrl.selectedDimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.selectedMeasures[0].name;
                    return queryFinalized;
                }

                function applyGroupKeysRules(selectedDimensions, dimensions)
                {


                    for (var i = 0; i < selectedDimensions.length; i++)
                    {
                        applyZoneRule(selectedDimensions[i], dimensions);
                        applyPortRule(selectedDimensions[i], dimensions);
                        applyCodeBuyRule(selectedDimensions[i], dimensions);
                        applyCodeSalesRule(selectedDimensions[i], dimensions);
                        applySupplierZoneIdRule(selectedDimensions[i], dimensions);
                    }
                    eliminateGroupKeysNotInParent(selectedDimensions, dimensions);
                }

                function applyZoneRule(selectedDimensions, dimensions)
                {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value)
                    {
                        var countryIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.Country.value, "value");
                        if (countryIndex != -1)
                            dimensions.splice(countryIndex, 1);
                        var codeBuyIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, "value");
                        if (codeBuyIndex != -1)
                            dimensions.splice(codeBuyIndex, 1);
                        var codeSalesIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, "value");
                        if (codeSalesIndex != -1)
                            dimensions.splice(codeSalesIndex, 1);
                    }
                }

                function applyPortRule(selectedDimensions, dimensions)
                {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.PortIn.value || selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.PortOut.value)
                    {
                        var gateWayOutIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.GateWayOut.value, "value");
                        if (gateWayOutIndex != -1)
                            dimensions.splice(gateWayOutIndex, 1);
                        var gateWayInIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.GateWayIn.value, "value");
                        if (gateWayInIndex != -1)
                            dimensions.splice(gateWayInIndex, 1);
                    }
                }

                function applyCodeBuyRule(selectedDimensions, dimensions)
                {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Country.value)
                        return;

                    var codeBuyIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, "value");
                    if (codeBuyIndex != -1)
                        dimensions.splice(codeBuyIndex, 1);
                }

                function applyCodeSalesRule(selectedDimensions, dimensions)
                {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Country.value)
                        return;
                    var codeSalesIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, "value");
                    if (codeSalesIndex != -1)
                        dimensions.splice(codeSalesIndex, 1);
                }

                function applySupplierZoneIdRule(selectedDimensions, dimensions)
                {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value)
                        return;

                    var supplierZoneIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value, "value");
                    if (supplierZoneIndex != -1)
                        dimensions.splice(supplierZoneIndex, 1);

                }

                function eliminateGroupKeysNotInParent(selectedDimensions, dimensions)
                {

                    for (var i = 0; i < selectedDimensions.length; i++)
                    {
                        for (var j = 0; j < dimensions.length; j++)
                            if (selectedDimensions[i].value == dimensions[j].value)
                                dimensions.splice(j, 1);
                    }
                }
            }



        }
        return directiveDefinitionObject;

    }
]);