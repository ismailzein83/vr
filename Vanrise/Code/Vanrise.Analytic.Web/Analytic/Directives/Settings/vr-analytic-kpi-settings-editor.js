'use strict';

app.directive('vrAnalyticKpiSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRUIUtilsService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new KPISettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Settings/Templates/KPISettingsTemplate.html"
        };

        function KPISettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var measureStyleRules;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.analyticTables = [];
                $scope.onAnalyticTableSelectorReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };

                $scope.onAnalyticTableSelectorChanged = function () {

                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        measureStyleRules = payload.data.AnalyticTablesKPISettings;
                    }
                    return VR_Analytic_AnalyticTableAPIService.GetAnalyticTablesInfo().then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                var analytictable = response[i];
                                var itemTab = {
                                    analytictable: analytictable,
                                };
                                extendDataItem(itemTab);
                            };
                        }
                    });
                };

                api.getData = function () {
                    var analyticTablesKPISettings = [];
                    for (var i = 0; i < $scope.analyticTables.length; i++) {
                        var analyticTable = $scope.analyticTables[i];
                        analyticTablesKPISettings.push({
                            AnalyticTableId: analyticTable.analyticTableId,
                            MeasureStyleRules: analyticTable.measureStyleGridAPI != undefined ? analyticTable.measureStyleGridAPI.getData() : undefined,
                            GlobalFilter: analyticTable.recordFilterAPI != undefined ? analyticTable.recordFilterAPI.getData().filterObj : undefined
                        });
                    };
                    var KPISettings = {
                        $type: "Vanrise.Analytic.Entities.KPISettings,Vanrise.Analytic.Entities",
                        AnalyticTablesKPISettings: analyticTablesKPISettings
                    };
                    return KPISettings;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendDataItem(itemTab) {
                console.log(itemTab);
                var measures = [];
                var dataItem = {
                    title: itemTab.analytictable.Name,
                    analyticTableId: itemTab.analytictable.AnalyticTableId,
                    isMeasureStyleGridLoading: true,
                    isRecordFilterLoading:true,
                    showKPISection: itemTab.analytictable.StatusDefinitionId != undefined? true : false
                };
                var input = {
                    TableIds: [itemTab.analytictable.AnalyticTableId],
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                };

                var context = {};
                context.getMeasures = function () {
                    var selectedMeasures = [];
                    for (var i = 0; i < measures.length; i++) {
                        selectedMeasures.push(measures[i]);
                    }
                    return selectedMeasures;
                };
                var analyticItemConfigPromise = VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var measureData = response[i];
                            var measure = {
                                FieldType: measureData.Config.FieldType,
                                Name: measureData.Name,
                                Title: measureData.Title,
                                AnalyticItemConfigId: measureData.AnalyticItemConfigId
                            };
                            measures.push(measure);
                        };
                    }
                });
                var measuresByAnalyticTable = UtilsService.getItemByVal(measureStyleRules, itemTab.analytictable.AnalyticTableId, "AnalyticTableId");
                dataItem.measureStyleLoadDeferred = UtilsService.createPromiseDeferred();
                dataItem.measureStyleLoadDeferred.promise.then(function () {
                    dataItem.isMeasureStyleGridLoading = false;
                });
                dataItem.onMeasureStyleGridReady = function (api) {
                    dataItem.measureStyleGridAPI = api;
                    analyticItemConfigPromise.then(function () {
                        var payload = {
                            context: context,
                            measureStyles: measuresByAnalyticTable != undefined ? measuresByAnalyticTable.MeasureStyleRules : undefined,
                            analyticTableId: itemTab.analytictable.AnalyticTableId
                        };
                        VRUIUtilsService.callDirectiveLoad(api, payload, dataItem.measureStyleLoadDeferred);
                    });
                };
                var fields = []
                var analyticTableDimensionsPromise = VR_Analytic_AnalyticItemConfigAPIService.GetDimensions(itemTab.analytictable.AnalyticTableId).then(function (response) {
                    if (response != undefined) {
                        var dimensions = response;
                        for (var i = 0; i < dimensions.length; i++) {
                            var dimension = response[i];
                            var field = {
                                FieldName: dimension.Name,
                                FieldTitle: dimension.Title,
                                Type: dimension.Config.FieldType,
                            };
                            fields.push(field);
                        };
                    }
                });
                var recordFilterContext = {
                    getFields: function () {
                        return fields;
                    },
                   
                };
                dataItem.recordFilterLoadDeferred = UtilsService.createPromiseDeferred();
                dataItem.recordFilterLoadDeferred.promise.then(function () {
                    dataItem.isRecordFilterLoading = false;
                });
                dataItem.onRecordFilterReady = function (api) {
                    dataItem.recordFilterAPI = api;
                    analyticTableDimensionsPromise.then(function () {
                        var payload = {
                            context: recordFilterContext,
                            FilterGroup: measuresByAnalyticTable != undefined ? measuresByAnalyticTable.GlobalFilter : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(api, payload, dataItem.recordFilterLoadDeferred);
                    });
                };
                $scope.analyticTables.push(dataItem);
            }

        }
    }]);