"use strict";

app.directive("vrWhsAnalyticsGenericgrid", ['UtilsService', 'VRNotificationService', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticAPIService', 'WhS_Analytics_GenericAnalyticMeasureEnum', 'VRUIUtilsService', 'WhS_Analytics_GenericAnalyticService', 'VRModalService',
    function (UtilsService, VRNotificationService,
        WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticAPIService, WhS_Analytics_GenericAnalyticMeasureEnum, VRUIUtilsService, WhS_Analytics_GenericAnalyticService, VRModalService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
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

        function GenericGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            ctrl.selectedMeasures = [];
            ctrl.selectedDimensions = [];
            ctrl.datasource = [];
            ctrl.selectedPeriods = [];
            ctrl.fromTime;
            ctrl.toTime;
            ctrl.dimensions = [];
            ctrl.dimensionFields = [];
            ctrl.parameters;
            var gridApi;
            var measureValues = [];
            var dimensionValues = [];
            var gridDrillDownTabsObj;
            var isSummary = false;

            function initializeController() {

                ctrl.mainGrid = (ctrl.parameters == undefined);

                ctrl.gridMenuActions = [{
                    name: "CDRs",
                    clicked: function (dataItem) {
                        var parameters = {
                            fromDate: UtilsService.cloneDateTime(ctrl.fromTime),
                            toDate: UtilsService.cloneDateTime(ctrl.toTime),
                            customerIds: [],
                            saleZoneIds: [],
                            supplierIds: [],
                            switchIds: [],
                            supplierZoneIds:[]
                        };

                        WhS_Analytics_GenericAnalyticService.updateParametersFromDimentions(parameters, ctrl, dataItem);
                        WhS_Analytics_GenericAnalyticService.showCdrLog(parameters);

                    }
                }];

                ctrl.gridLeftMenuActions = [
                {
                    name: "Settings",
                    onClicked: editSettings
                }];
                ctrl.getColor = function (dataItem, coldef) {
                    if (ctrl.parameters != undefined)
                        return WhS_Analytics_GenericAnalyticService.getMeasureColor(dataItem, coldef, ctrl.parameters);
                };

             

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (query) {
                            ctrl.filters = query.Filters;
                            var filters = query.Filters;
                            ctrl.Currency = query.Currency;
                            dimensionValues.length = 0;
                            ctrl.selectedDimensions.length = 0;
                            ctrl.selectedPeriods.length = 0;
                            ctrl.selectedMeasures.length = 0;
                            ctrl.dimensions.length = 0;
                            ctrl.dimensionFields.length = 0;

                            var queryFinalized = loadGridQuery(query);

                            var drillDownDefinitions = [];

                            applyDimentionsRules(ctrl.selectedDimensions, ctrl.dimensions);

                            for (var i = 0; i < ctrl.dimensions.length; i++) {
                                var selectedDimensions = [];
                                var dimention = ctrl.dimensions[i];
                                for (var j = 0; j < ctrl.selectedDimensions.length; j++)
                                    if (ctrl.selectedDimensions[j].value != dimention.value)
                                        selectedDimensions.push(ctrl.selectedDimensions[j].value);
                                setDrillDownData(ctrl.dimensions[i], selectedDimensions)
                            }

                            function setDrillDownData(dimention, selectedDimensions) {
                                var objData = {};

                                objData.title = dimention.name;

                                objData.directive = "vr-whs-analytics-genericgrid";

                                objData.loadDirective = function (directiveAPI, dataItem) {

                                    var selectedfilters = [];
                                    for (var j = 0; j < ctrl.dimensionFields.length; j++) {
                                        selectedfilters.push({
                                            Dimension: ctrl.dimensionFields[j].value,
                                            FilterValues: [dataItem.DimensionValues[j].Id]
                                        });
                                    }

                                    for (var i = 0; i < filters.length; i++)
                                        selectedfilters.push(filters[i]);

                                    dataItem.gridAPI = directiveAPI;

                                    var query = {
                                        Filters: selectedfilters,
                                        DimensionFields: [dimention.value],
                                        MeasureFields: measureValues,
                                        DimensionsSelected: selectedDimensions,
                                        MeasureThreshold: ctrl.parameters,
                                        FromTime: ctrl.fromTime,
                                        ToTime: ctrl.toTime,
                                        Currency: ctrl.Currency,
                                        WithSummary: isSummary
                                    };
                                    return dataItem.gridAPI.loadGrid(query);
                                };

                                drillDownDefinitions.push(objData);
                            }

                            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, ctrl.gridMenuActions);

                            return gridApi.retrieveData(queryFinalized);

                        };

                        return directiveAPI;
                    }

                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    ctrl.showGrid = true;

                    return WhS_Analytics_GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                                if (isSummary)
                                    gridApi.setSummary(response.Summary);
                            }
                           

                            onResponseReady(response);
                        });
                };

                ctrl.checkExpandablerow = function (groupKeys) {
                    return groupKeys.length !== ctrl.groupKeys.length;
                };

                function loadGridQuery(query) {
                    dimensionValues.length = 0;
                    ctrl.fromTime = query.FromTime;
                    ctrl.toTime = query.ToTime;
                    if (query.MeasureThreshold != undefined) {
                        ctrl.parameters = query.MeasureThreshold;
                        ctrl.mainGrid = false;
                    }

                    if (query.DimensionsSelected != undefined) {
                        for (var i = 0; i < query.DimensionsSelected.length; i++) {
                            var enumObj = getEnumIfContain(query.DimensionsSelected[i]);
                            if (enumObj != undefined)
                                ctrl.selectedDimensions.push(enumObj);
                        }
                    }

                    if (query.DimensionFields != undefined) {
                        for (var i = 0; i < query.DimensionFields.length; i++) {
                            var enumObj = getEnumIfContain(query.DimensionFields[i]);
                            if (enumObj != undefined)
                            {
                                ctrl.dimensionFields.push(enumObj);
                                ctrl.selectedDimensions.push(enumObj);
                            }
                        }
                        dimensionValues = query.DimensionFields;
                    }

                    if (query.FixedDimensionFields != undefined) {
                        var enumObj = getEnumIfContain(query.FixedDimensionFields);
                        if (enumObj != undefined)
                        {
                            ctrl.dimensionFields.push(enumObj);
                            ctrl.selectedDimensions.push(enumObj);
                            ctrl.selectedPeriods.push(enumObj);
                            dimensionValues.push(query.FixedDimensionFields);
                            if (query.FixedDimensionFields == WhS_Analytics_GenericAnalyticDimensionEnum.Hour.value) {
                                ctrl.dimensionFields.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                dimensionValues.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date.value);
                                ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                ctrl.selectedPeriods.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                            }
                        }
                    }

                    for (var p in WhS_Analytics_GenericAnalyticDimensionEnum) {
                      if(WhS_Analytics_GenericAnalyticDimensionEnum[p].isDimention == true)
                        ctrl.dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                    }

                    if (query.MeasureFields != undefined) {
                        for (var i = 0; i < query.MeasureFields.length; i++)
                            for (var p in WhS_Analytics_GenericAnalyticMeasureEnum)
                                if (WhS_Analytics_GenericAnalyticMeasureEnum[p].value == query.MeasureFields[i])
                                    ctrl.selectedMeasures.push(WhS_Analytics_GenericAnalyticMeasureEnum[p]);
                        measureValues = query.MeasureFields;
                    }

                    isSummary = $attrs.withsummary != undefined;
                    var queryFinalized = {
                        Filters: query.Filters,
                        DimensionFields: dimensionValues,
                        MeasureFields: query.MeasureFields,
                        FromTime: query.FromTime,
                        ToTime: query.ToTime,
                        Currency: query.Currency,
                        WithSummary: isSummary
                    };
                    if (ctrl.selectedPeriods.length > 0 || ctrl.selectedDimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.selectedMeasures[0].name;
                    return queryFinalized;
                }

                function getEnumIfContain(comparativeValue) {
                    for (var p in WhS_Analytics_GenericAnalyticDimensionEnum) {
                        if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == comparativeValue) {
                            return WhS_Analytics_GenericAnalyticDimensionEnum[p];
                        }
                       
                    }
                    return undefined;
                }

                function applyDimentionsRules(selectedDimensions, dimensions) {
                    var supplierZoneIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value, dimensions);
                    if (supplierZoneIndex != -1)
                        dimensions.splice(supplierZoneIndex, 1);
                    if (getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, dimensions) == -1)
                        dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales);
                    if (getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, dimensions) == -1)
                        dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy);

                    for (var i = 0; i < selectedDimensions.length; i++) {

                        switch (selectedDimensions[i].value) {
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value:
                                var countryIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.Country.value, dimensions);
                                if (countryIndex != -1)
                                    dimensions.splice(countryIndex, 1);
                                var codeBuyIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, dimensions);
                                if (codeBuyIndex != -1)
                                    dimensions.splice(codeBuyIndex, 1);
                                var codeSalesIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, dimensions);
                                if (codeSalesIndex != -1)
                                    dimensions.splice(codeSalesIndex, 1);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value:
                                if (getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value, dimensions) == -1)
                                    dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone);
                                break;
                                //case WhS_Analytics_GenericAnalyticDimensionEnum.PortIn.value || WhS_Analytics_GenericAnalyticDimensionEnum.PortOut.value:
                                //    var gateWayOutIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.GateWayOut.value);
                                //            if (gateWayOutIndex != -1)
                                //                dimensions.splice(gateWayOutIndex, 1);
                                //            var gateWayInIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.GateWayIn.value);
                                //            if (gateWayInIndex != -1)
                                //                dimensions.splice(gateWayInIndex, 1);
                                //            break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Country.value:
                                var codeBuyIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, dimensions);
                                if (codeBuyIndex != -1)
                                    dimensions.splice(codeBuyIndex, 1);
                                var codeSalesIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, dimensions);
                                if (codeSalesIndex != -1)
                                    dimensions.splice(codeSalesIndex, 1);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value:
                                var countryIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.Country.value, dimensions);
                                if (countryIndex != -1)
                                    dimensions.splice(countryIndex, 1);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value:
                                var countryIndex = getDimentionIndex(WhS_Analytics_GenericAnalyticDimensionEnum.Country.value, dimensions);
                                if (countryIndex != -1)
                                    dimensions.splice(countryIndex, 1);
                                break;

                        }
                    }
                    eliminateGroupKeysNotInParent(selectedDimensions, dimensions);
                }

                function getDimentionIndex(dimentionValue, dimensions) {
                    return UtilsService.getItemIndexByVal(dimensions, dimentionValue, "value");
                }

                function eliminateGroupKeysNotInParent(selectedDimensions, dimensions) {

                    for (var i = 0; i < selectedDimensions.length; i++) {
                        for (var j = 0; j < dimensions.length; j++)
                            if (selectedDimensions[i].value == dimensions[j].value)
                                dimensions.splice(j, 1);
                    }
                }


                function  editSettings () {
                    var settings = {
                    };

                    settings.onScopeReady = function (modalScope) {
                        modalScope.title = UtilsService.buildTitleForUpdateEditor("Measure Threshold");
                        modalScope.onSaveSettings = function (parameters) {
                            ctrl.parameters = parameters
                        };
                    };
                    var measureThreshold = [];
                    for (var i = 0; i < ctrl.selectedMeasures.length; i++) {
                        switch (ctrl.selectedMeasures[i].value) {
                            case WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value: measureThreshold.push(WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value); break;
                            case WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value: measureThreshold.push(WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value); break;
                            case WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value: measureThreshold.push(WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value); break;
                        }
                    }
                    var parameters = {
                        measureThresholds: measureThreshold
                    };

                    VRModalService.showModal('/Client/Modules/WhS_Analytics/Directives/Generic/Templates/GenericAnalyticGridSettings.html', parameters, settings);
                }
            }

        }
        return directiveDefinitionObject;

    }
]);