"use strict";

app.directive("vrAnalyticDatagridAnalyticrecords", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService', 'DataGridRetrieveDataEventType','VR_Analytic_StyleCodeEnum','Analytic_AnalyticService','VR_Analytic_GridWidthEnum',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, DataGridRetrieveDataEventType, VR_Analytic_StyleCodeEnum, Analytic_AnalyticService, VR_Analytic_GridWidthEnum) {

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
            controllerAs: "analyticrecordsctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/Grid/Templates/AnalyticRecordsDataGrid.html"

        };

        function GenericGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            ctrl.datasource = [];
            ctrl.dimensions = [];
            ctrl.groupingDimensions = [];
            ctrl.dimensionsConfig = [];
            ctrl.measuresConfig = [];
            ctrl.selectedDimensions = [];
            ctrl.parentDrillDownDimensions = [];
            ctrl.parentDimensions = [];
            ctrl.measures = [];
            var initialQueryOrderType;
            var gridPayload;
            var measureStyleRules;
            ctrl.drillDownDimensions = [];

            ctrl.sortField = "";
            var gridApi;
            var tableId;
            var drillDown;
            var fromTime;
            var toTime;
            var gridWidths;
            function initializeController() {

                gridWidths = UtilsService.getArrayEnum(VR_Analytic_GridWidthEnum);
                ctrl.mainGrid = (ctrl.parameters == undefined);
                var styleColors = UtilsService.getArrayEnum(VR_Analytic_StyleCodeEnum);
                ctrl.getMeasureColor = function (dataItem, colDef) {
                    var measure = dataItem.MeasureValues[colDef.tag];
                    if (measure != undefined) {
                        var style = UtilsService.getItemByVal(styleColors, measure.StyleCode, 'value');
                        if (style !=undefined)
                            return style.styleCode;
                    }
                }

                ctrl.gridLeftMenuActions = [
                {
                    name: "Settings",
                    onClicked: editSettings
                }];

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.load = function (payLoad) {
                            var promiseReadyDeferred = UtilsService.createPromiseDeferred();
                            tableId = payLoad.TableId;
                            var promises = [];
                            if (payLoad.DimensionsConfig == undefined) {
                                promises.push(loadDimensionsConfig());
                            }
                            if (payLoad.MeasuresConfig == undefined) {
                                promises.push(loadMeasuresConfig());
                            }
                            UtilsService.waitMultiplePromises(promises).then(function () {
                                loadGrid(payLoad).finally(function () {
                                    promiseReadyDeferred.resolve();
                                }).catch(function (error) {
                                    promiseReadyDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                promiseReadyDeferred.reject(error);
                            });;
                            return promiseReadyDeferred.promise;

                        }

                        return directiveAPI;
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady, retrieveDataContext) {
            
                    ctrl.showGrid = true;
                    if (!retrieveDataContext.isDataSorted)
                        dataRetrievalInput.Query.OrderType = initialQueryOrderType;
                    else
                        dataRetrievalInput.Query.OrderType = undefined;
                    if (dataRetrievalInput.Query.WithSummary && retrieveDataContext.eventType != DataGridRetrieveDataEventType.ExternalTrigger)
                        dataRetrievalInput.Query.WithSummary = false;

                    return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                if (ctrl.drillDownDimensions.length > 0) {
                                    for (var i = 0; i < response.Data.length; i++) {

                                        drillDown.setDrillDownExtensionObject(response.Data[i]);
                                    }
                                }

                                if (retrieveDataContext.eventType == DataGridRetrieveDataEventType.ExternalTrigger && dataRetrievalInput.Query.WithSummary) {
                                    if (response.Summary != undefined)
                                        gridApi.setSummary(response.Summary);
                                    else {
                                        gridApi.clearSummary();
                                    }
                                }
                            }
                            onResponseReady(response);
                        });
                };


                function editSettings() {
                    var onSaveSettings = function (settings) {
                        measureStyleRules = settings;
                        if (gridPayload !=undefined)
                        {
                            if(gridPayload.Settings !=undefined)
                                gridPayload.Settings.MeasureStyleRules = settings.MeasureStyleRules;

                            var payload = {
                                Settings: gridPayload.Settings,
                                DimensionFilters: gridPayload.DimensionFilters,
                                SelectedGroupingDimensions: gridPayload.SelectedGroupingDimensions,
                                TableId: gridPayload.TableId,
                                FromTime: fromTime,
                                FilterGroup: gridPayload.FilterGroup,
                                ToTime: toTime
                            }
                            loadGrid(payload);
                        }
                       
                    };
                    var measureStyleRules;
                    Analytic_AnalyticService.openGridWidgetSettings(onSaveSettings, getSettingContext(), measureStyleRules);
                }


                // ------- Load Grid ------

                function loadGrid(payLoad) {
                    ctrl.parentDimensions.length = 0;
                    ctrl.drillDownDimensions.length = 0;
                    var filters = payLoad.DimensionFilters;
                    var queryFinalized = loadGridQuery(payLoad);

                    addGridAttributesForDimensions(ctrl.groupingDimensions);

                    addGridAttributesForMeasures(ctrl.measures);




                    var drillDownDefinitions = [];

                    for (var i = 0; i < ctrl.drillDownDimensions.length; i++) {
                        var selectedDimensions = [];
                        var dimension = ctrl.drillDownDimensions[i];
                        for (var j = 0; j < ctrl.groupingDimensions.length; j++)
                            if (ctrl.groupingDimensions[j].DimensionName != dimension.DimensionName)
                                selectedDimensions.push(ctrl.groupingDimensions[j].DimensionName);
                        if (!dimension.hideDimension)
                            setDrillDownData(dimension, selectedDimensions)
                    }

                    function setDrillDownData(dimension, selectedDimensions) {
                        var objData = {};
                        objData.title = dimension.Title;
                        var drillDownDimensions = [];
                        for (var i = 0; i < ctrl.drillDownDimensions.length; i++) {
                            drillDownDimensions.push(ctrl.drillDownDimensions[i]);
                        }

                        var parentDimensions = [];
                        for (var i = 0; i < ctrl.parentDimensions.length; i++) {
                            if (UtilsService.getItemByVal(parentDimensions, ctrl.parentDimensions[i].DimensionName, 'DimensionName') == undefined)
                                parentDimensions.push(ctrl.parentDimensions[i]);
                        }
                        if (UtilsService.getItemByVal(parentDimensions, dimension.DimensionName, 'DimensionName') == undefined)
                            parentDimensions.push(dimension);
                        if (UtilsService.contains(drillDownDimensions, dimension)) {
                            var dimensionIndex = UtilsService.getItemIndexByVal(drillDownDimensions, dimension.DimensionName, 'DimensionName');
                            drillDownDimensions.splice(dimensionIndex, 1);
                        }

                        objData.directive = "vr-analytic-datagrid-analyticrecords";

                        objData.loadDirective = function (directiveAPI, dataItem) {

                            dataItem.gridAPI = directiveAPI;
                            //UpdateFilters
                            var newFilters = [];
                            for (var j = 0; j < ctrl.groupingDimensions.length; j++) {
                                newFilters.push({
                                    Dimension: ctrl.groupingDimensions[j].DimensionName,
                                    FilterValues: [dataItem.DimensionValues[j].Value]
                                });
                            }
                            for (var i = 0; i < filters.length; i++)
                                newFilters.push(filters[i]);

                            //Remove Current Dimension from DrillDownDimensions

                            var drillDownPayLoad = {
                                ParentDrillDownDimensions: ctrl.drillDownDimensions,
                                ParentDimensions: parentDimensions,
                                DimensionsConfig: ctrl.dimensionsConfig,
                                MeasuresConfig: ctrl.measuresConfig,
                                GroupingDimensions: [dimension],
                                Dimensions: ctrl.dimensions,
                                DimensionFilters: newFilters,
                                Measures: ctrl.measures,
                                FromTime: fromTime,
                                ToTime: toTime,
                                DrillDownDimensions: drillDownDimensions,
                                TableId: payLoad.TableId,
                                InitialQueryOrderType: initialQueryOrderType,
                                MeasureStyleRules: measureStyleRules
                            }
                            return dataItem.gridAPI.load(drillDownPayLoad);
                        };

                        drillDownDefinitions.push(objData);
                    }

                    drillDown = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, undefined);
                    return gridApi.retrieveData(queryFinalized);
                }

                function loadGridQuery(payLoad) {
                    ctrl.groupingDimensions.length = 0;
                    ctrl.measures.length = 0;
                    ctrl.drillDownDimensions.length = 0;

                    fromTime = payLoad.FromTime;
                    toTime = payLoad.ToTime;

                    // Load Data From Root Grid
                    if (payLoad.Settings != undefined) {
                        loadDataFromRootGrid(payLoad);
                    }
                        // End
                        // Load Data From Child Grid
                    else {
                        loadDataFromChildGrid(payLoad);

                    }

                    // End

                    if (ctrl.groupingDimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0] + '.Value';

                    var queryFinalized = {
                        Filters: payLoad.DimensionFilters,
                        DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                        MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                        FromTime: fromTime,
                        ToTime: toTime,
                        FilterGroup: payLoad.FilterGroup,
                        Currency: payLoad.Currency,
                        ParentDimensions: UtilsService.getPropValuesFromArray(ctrl.parentDimensions, 'DimensionName'),
                        WithSummary: payLoad.Settings == undefined ? false : payLoad.Settings.WithSummary,
                        TableId: payLoad.TableId,
                        MeasureStyleRules: measureStyleRules
                    }
                    return queryFinalized;
                }

                function loadDataFromRootGrid(payload) {
                   
                    ctrl.dimensions.length = 0;
                    ctrl.groupingDimensions.length = 0;
                    loadAllGridDimensions(payload);
                    var dimensionForDrillDown = [];
                    for (var i = 0; i < ctrl.dimensions.length; i++) {
                        var dimension = ctrl.dimensions[i];
                        var gridWidth = UtilsService.getItemByVal(gridWidths, dimension.Width, "value");
                        if (gridWidth != undefined)
                            dimension.Widthfactor = gridWidth.widthFactor;


                        if (payload.Settings != undefined) {
                            gridPayload = payload;
                            measureStyleRules = payload.Settings.MeasureStyleRules;
                            initialQueryOrderType = payload.Settings.OrderType;
                            if (payload.Settings.RootDimensionsFromSearchSection) {
                                var groupingSearchDimension = UtilsService.getItemByVal(payload.SelectedGroupingDimensions, dimension.DimensionName, 'DimensionName');
                                if (groupingSearchDimension != undefined && UtilsService.getItemByVal(ctrl.groupingDimensions, dimension.DimensionName, 'DimensionName') == undefined)
                                    ctrl.groupingDimensions.push(dimension);
                            }
                            if (payload.Settings.Dimensions != undefined) {
                                var groupingDimension = UtilsService.getItemByVal(payload.Settings.Dimensions, dimension.DimensionName, 'DimensionName');
                                if (groupingDimension.IsRootDimension && UtilsService.getItemByVal(ctrl.groupingDimensions, dimension.DimensionName, 'DimensionName') == undefined) {
                                    ctrl.groupingDimensions.push(dimension);
                                } else if (!groupingDimension.IsRootDimension && UtilsService.getItemByVal(ctrl.groupingDimensions, dimension.DimensionName, 'DimensionName') == undefined) {

                                    dimensionForDrillDown.push(dimension);
                                }
                            }

                        }
                    }

                    for (var i = 0; i < ctrl.groupingDimensions.length; i++) {
                        if (UtilsService.getItemByVal(ctrl.parentDimensions, ctrl.groupingDimensions[i].DimensionName, 'DimensionName') == undefined)
                            ctrl.parentDimensions.push(ctrl.groupingDimensions[i]);
                    }
                    applyDimensionRules(dimensionForDrillDown);

                    loadMeasures(payload);
                }

                function loadDataFromChildGrid(payload) {

                    initialQueryOrderType = payload.InitialQueryOrderType;
                    if (payload.Dimensions != undefined) {
                        ctrl.dimensions = payload.Dimensions;
                    }
                    if (payload.MeasureStyleRules !=undefined)
                        measureStyleRules = payload.MeasureStyleRules
                    if (payload.DimensionsConfig != undefined) {
                        ctrl.dimensionsConfig = payload.DimensionsConfig;
                    }
                    if (payload.MeasuresConfig != undefined) {
                        ctrl.measuresConfig = payload.MeasuresConfig;
                    }
                    if (payload.GroupingDimensions != undefined && payload.Settings == undefined) {
                        for (var i = 0; i < payload.GroupingDimensions.length; i++) {
                            var groupingDimension = payload.GroupingDimensions[i];
                            var dimension = UtilsService.getItemByVal(ctrl.dimensions, groupingDimension.DimensionName, 'DimensionName');
                            var gridWidth = UtilsService.getItemByVal(gridWidths, dimension.Width, "value");
                            if (gridWidth != undefined)
                                dimension.Widthfactor = gridWidth.widthFactor;
                            ctrl.groupingDimensions.push(dimension);
                           
                        }
                    }
                    if (payload.ParentDimensions != undefined)
                        ctrl.parentDimensions = payload.ParentDimensions;

                    loadMeasures(payload);
                    if (payload.ParentDrillDownDimensions != undefined) {
                        applyDimensionRules(payload.ParentDrillDownDimensions)
                        ctrl.parentDrillDownDimensions = payload.ParentDrillDownDimensions;
                    }
                }

                function loadMeasures(payload) {

                    if (payload.Settings != undefined && payload.Settings.Measures != undefined) {
                        for (var i = 0; i < payload.Settings.Measures.length; i++) {
                            var settingMeasure = payload.Settings.Measures[i];
                            var gridWidth = UtilsService.getItemByVal(gridWidths, settingMeasure.Width, "value");
                            if (gridWidth != undefined)
                                settingMeasure.Widthfactor = gridWidth.widthFactor;
                            ctrl.measures.push(settingMeasure);
                        }
                    } else if (payload.Measures != undefined) {
                        for (var i = 0; i < payload.Measures.length; i++) {
                            var measure = payload.Measures[i];
                            var gridWidthValue = UtilsService.getItemByVal(gridWidths, measure.Width, "value");
                            if (gridWidthValue != undefined)
                                measure.Widthfactor = gridWidthValue.widthFactor;
                            ctrl.measures.push(measure);
                        }
                    }
                }

                function loadAllGridDimensions(payload) {
                    var allDimensions = [];
                    if (payload.Settings != undefined) {
                        for (var i = 0; i < payload.Settings.Dimensions.length; i++) {
                            var dimension = payload.Settings.Dimensions[i];
                            allDimensions.push(dimension);
                        }
                    }
                    ctrl.dimensions = allDimensions;
                }

                // ------- End Load Grid ------


                //------- Rules ------------

                function applyDimensionRules(parentDrillDownDimensions) {
                    for (var i = 0; i < parentDrillDownDimensions.length; i++) {
                        var drillDownDimension = parentDrillDownDimensions[i];
                        var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, drillDownDimension.DimensionName, 'Name');

                        if (dimensionConfig != undefined) {
                            if (dimensionConfig.RequiredParentDimension == undefined || (dimensionConfig.RequiredParentDimension != undefined && (UtilsService.getItemByVal(ctrl.groupingDimensions, dimensionConfig.RequiredParentDimension, "DimensionName") != undefined || UtilsService.getItemByVal(ctrl.parentDimensions, dimensionConfig.RequiredParentDimension, "DimensionName") != undefined))) {
                                drillDownDimension.hideDimension = false;
                            } else {
                                drillDownDimension.hideDimension = true;
                            }
                            if (!checkIfExitsInGroupDimensions(drillDownDimension.DimensionName) && checkIfShouldAddParent(drillDownDimension.DimensionName, ctrl.groupingDimensions))
                                ctrl.drillDownDimensions.push(drillDownDimension);
                        }
                    }
                }

                function checkIfExitsInGroupDimensions(dimensionName) {
                    return UtilsService.getItemByVal(ctrl.groupingDimensions, dimensionName, "DimensionName");
                }

                function checkIfShouldAddParent(dimensionName, groupingDimensions) {
                    for (var i = 0; i < groupingDimensions.length; i++) {
                        var groupingDimension = groupingDimensions[i];

                        var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, groupingDimension.DimensionName, 'Name');
                        if (dimensionConfig != undefined) {
                            return checkIfShouldAddParentRecursively(dimensionName, dimensionConfig.Name, dimensionConfig.Parents);
                        }
                    }

                }

                function checkIfShouldAddParentRecursively(dimensionName, groupingDimensionName, parents) {

                    if (parents == undefined || parents.length == 0)
                        return true;
                    var result = false;
                    for (var i = 0; i < parents.length; i++) {
                        var parent = parents[i];

                        if (parent == dimensionName && UtilsService.getItemByVal(ctrl.groupingDimensions, parent, "DimensionName") == undefined)
                            return false;
                        else {
                            var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, parent, 'Name');
                            if (dimensionConfig != undefined) {
                                result = checkIfShouldAddParentRecursively(dimensionName, groupingDimensionName, dimensionConfig.Parents);
                                if (!result)
                                    return result;
                            }

                        }
                    }
                    return result;
                }

                //------- END Rules ------------


                //------- Load Configs ------------

                function loadDimensionsConfig() {
                    var dimensionsFilter = {
                        TableIds: [tableId]
                    }
                    return VR_Analytic_AnalyticItemConfigAPIService.GetDimensionsInfo(UtilsService.serializetoJson(dimensionsFilter)).then(function (response) {

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dimensionsConfig.push(response[i]);
                            }
                        };
                    });
                }

                function loadMeasuresConfig() {
                    var measuresFilter = {
                        TableIds: [tableId]
                    }
                    return VR_Analytic_AnalyticItemConfigAPIService.GetMeasuresInfo(UtilsService.serializetoJson(measuresFilter)).then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.measuresConfig.push(response[i]);
                            }
                        };
                    });
                }

                //------- END Load Configs ------------


                //------- Add Grid Attributes ------------

                function addGridAttributesForDimensions(groupingDimensions) {
                    for (var i = 0; i < groupingDimensions.length; i++) {
                        var groupingDimenion = groupingDimensions[i];
                        var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, groupingDimenion.DimensionName, "Name");
                        if (dimensionConfig != undefined) {
                            groupingDimenion.Type = dimensionConfig.Attribute.Type;
                            groupingDimenion.NumberPrecision = dimensionConfig.Attribute.NumberPrecision;
                        }
                    }
                }

                function addGridAttributesForMeasures(measures) {
                    for (var i = 0; i < measures.length; i++) {
                        var measure = measures[i];
                        var measureConfig = UtilsService.getItemByVal(ctrl.measuresConfig, measure.MeasureName, "Name");
                        if (measureConfig != undefined) {
                            measure.Type = measureConfig.Attribute.Type;
                            measure.NumberPrecision = measureConfig.Attribute.NumberPrecision;

                        }
                    }
                }

                //------- END Add Grid Attributes ------------

                function getSettingContext()
                {
                    var context = {
                        getMeasures: function () {
                            var selectedMeasures = [];
                            for (var i = 0; i < ctrl.measures.length; i++) {
                                var selectedMeasure = ctrl.measures[i];
                                var matchItem = UtilsService.getItemByVal(ctrl.measuresConfig, selectedMeasure.MeasureName, "Name");
                                if (matchItem != undefined) {
                                    selectedMeasures.push({
                                        Name: selectedMeasure.MeasureName,
                                        Title: selectedMeasure.Title,
                                        FieldType: matchItem.FieldType
                                    });
                                }
                            }
                            return selectedMeasures;

                        }
                    }
                    return context;
                }
            }
        }
        return directiveDefinitionObject;
    }
]);