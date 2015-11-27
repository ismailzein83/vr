"use strict";

app.directive("vrWhsAnalyticsGenericgrid", ['UtilsService', 'VRNotificationService', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticAPIService', 'WhS_Analytics_GenericAnalyticMeasureEnum','VRUIUtilsService',
function ( UtilsService, VRNotificationService,
        WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticAPIService, WhS_Analytics_GenericAnalyticMeasureEnum, VRUIUtilsService) {

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
        ctrl.selectedfilters = [];
        ctrl.selectedPeriods = [];
        ctrl.fromTime;
        ctrl.toTime;
        ctrl.dimensions = [];

        ctrl.dimensionFields = [];
        var gridApi;
        var measureValues = [];
        var dimensionValues = [];
        var gridDrillDownTabsObj;
        function initializeController() {
            

            ctrl.gridReady = function(api) {
                gridApi = api;
                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        dimensionValues.length = 0;
                        ctrl.selectedDimensions.length = 0;
                       // ctrl.selectedfilters.length = 0;
                        ctrl.selectedPeriods.length = 0;
                        ctrl.selectedMeasures.length = 0;
                        ctrl.dimensions.length = 0;
                        ctrl.dimensionFields.length = 0;
                        var queryFinalized = loadGridQuery(query);

                        var drillDownDefinitions = [];

                        applyGroupKeysRules(ctrl.selectedDimensions, ctrl.dimensions)

                        for (var i = 0; i < ctrl.dimensions.length; i++) {
                            var array=[];
                            var obj = ctrl.dimensions[i];
                            for (var j = 0; j < ctrl.selectedDimensions.length; j++)
                                if (ctrl.selectedDimensions[j].value != obj.value)
                                    array.push(ctrl.selectedDimensions[j].value);
                            setDrillDownData(ctrl.dimensions[i], array, i)
                            
                        }

                       

                        function setDrillDownData(obj, array,index) {
                            var objData = {};
                            objData.title= obj.name;
                            objData.directive= "vr-whs-analytics-genericgrid";
                            objData.loadDirective = function (directiveAPI, dataItem) {
                                    //ctrl.selectedfilters.push({
                                    //    Dimension: obj.value,
                                    //    FilterValues: [dataItem.DimensionValues[index].Id]
                                    //});

                                dataItem.gridAPI = directiveAPI;
                                dimensionValues
                                var query = {
                                    Filters: ctrl.selectedfilters,
                                    DimensionFields: [obj.value],
                                    MeasureFields: measureValues,
                                    DimensionsSelected: array,
                                    FromTime: ctrl.fromTime,
                                    ToTime: ctrl.toTime,
                                    Currency: undefined,
                                    WithSummary: $attrs.withsummary != undefined
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

            ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                ctrl.showGrid = true;
                return WhS_Analytics_GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {
                    if (response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                   

                    //if (isSummary)
                    //    gridApi.setSummary(response.Summary);

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
                for (var p in WhS_Analytics_GenericAnalyticDimensionEnum)
                {

                    ctrl.dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                    if (query.DimensionsSelected != undefined)
                    {
                        for (var i = 0; i < query.DimensionsSelected.length; i++)
                        {
                            if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == query.DimensionsSelected[i])
                                ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
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
                            ctrl.selectedPeriods.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                            dimensionValues.push(query.FixedDimensionFields);
                            if (query.FixedDimensionFields == WhS_Analytics_GenericAnalyticDimensionEnum.Hour.value)
                            {
                                ctrl.dimensionFields.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
                                dimensionValues.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date.value);
                                ctrl.selectedDimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date);
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

                var queryFinalized = {
                    Filters: query.Filters,
                    DimensionFields: dimensionValues,
                    MeasureFields: query.MeasureFields,
                    FromTime: query.FromTime,
                    ToTime: query.ToTime,
                    Currency: query.Currency,
                    WithSummary: $attrs.withsummary != undefined
                }
                if (ctrl.selectedPeriods.length > 0 || ctrl.selectedDimensions.length > 0)
                    ctrl.sortField = 'DimensionValues[0].Name';
                else
                    ctrl.sortField = 'MeasureValues.' + ctrl.selectedMeasures[0].name;
                return queryFinalized;
            }

            function applyGroupKeysRules(selectedDimensions, dimensions) {


                for (var i = 0; i < selectedDimensions.length; i++) {
                    applyZoneRule(selectedDimensions[i], dimensions);
                    applyPortRule(selectedDimensions[i], dimensions);
                    applyCodeBuyRule(selectedDimensions[i], dimensions);
                    applyCodeSalesRule(selectedDimensions[i], dimensions);
                    applySupplierZoneIdRule(selectedDimensions[i], dimensions);
                }
                eliminateGroupKeysNotInParent(selectedDimensions, dimensions);
            }
            function applyZoneRule(selectedDimensions, dimensions) {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value) {
                        var gatCountryIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeGroup.value, "value");
                        if (gatCountryIndex != -1)
                        dimensions.splice(gatCountryIndex, 1);
                        var gatCodeBuyIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, "value");
                        if (gatCodeBuyIndex != -1)
                        dimensions.splice(gatCodeBuyIndex, 1);
                        var gateCodeSalesIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, "value");
                        if (gateCodeSalesIndex != -1)
                        dimensions.splice(gateCodeSalesIndex, 1);
                }
            }
            function applyPortRule(selectedDimensions, dimensions) {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.PortIn.value || selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.PortOut.value) {
                        var gateWayOutIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.GateWayOut.value, "value");
                        if (gateWayOutIndex != -1)
                        dimensions.splice(gateWayOutIndex, 1);
                        var gateWayInIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.GateWayIn.value, "value");
                        if (gateWayInIndex != -1)
                        dimensions.splice(gateWayInIndex, 1);
                }
            }
            function applyCodeBuyRule(selectedDimensions, dimensions) {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.CodeGroup.value)
                        return;

                    var gatCodeBuyIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value, "value");
                    if (gatCodeBuyIndex!=-1)
                      dimensions.splice(gatCodeBuyIndex, 1);
            }
            function applyCodeSalesRule(selectedDimensions, dimensions) {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.CodeGroup.value)
                        return;
                    var gateCodeSalesIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value, "value");
                    if (gateCodeSalesIndex != -1)
                    dimensions.splice(gateCodeSalesIndex, 1);
            }
            function applySupplierZoneIdRule(selectedDimensions, dimensions) {
                    if (selectedDimensions.value == WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value)
                        return;

                    var gateSupplierZoneIndex = UtilsService.getItemIndexByVal(dimensions, WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value, "value");
                    if (gateSupplierZoneIndex != -1)
                        dimensions.splice(gateSupplierZoneIndex, 1);

            }

            function eliminateGroupKeysNotInParent(selectedDimensions, dimensions) {

                for (var i = 0; i < selectedDimensions.length; i++) {
                    for (var j = 0; j < dimensions.length; j++)
                        if (selectedDimensions[i].value == dimensions[j].value)
                            dimensions.splice(j, 1);
                }
            }
        }

       
      
        }
    return directiveDefinitionObject;

}]);
