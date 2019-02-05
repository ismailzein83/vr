(function (app) {

    'use strict';

    WidgetSettingsDefinition.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'VR_Analytic_AnalyticTypeEnum', 'UtilsService', 'VRUIUtilsService', 'ColumnWidthEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function WidgetSettingsDefinition(VR_Analytic_AnalyticConfigurationAPIService, VR_Analytic_AnalyticTypeEnum, UtilsService, VRUIUtilsService, ColumnWidthEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var widgets = new Widgets($scope, ctrl, $attrs);
                widgets.initializeController();
            },
            controllerAs: "searchSettingsCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/WidgetSettings/Templates/WidgetSettingsDefinitionTemplate.html"
        };

        function Widgets($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var tableIds;
            var dimensions;
            var settings;

            var periodSelectorAPI;
            var periodSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectorAPI;

            var timePeriod;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var tableSelectorSelectionChanged;
            var tableSelectedPromiseDeffered;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                };
                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onPeriodSelectorReady = function (api) {
                    periodSelectorAPI = api;
                    periodSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectionTableChanged = function () {
                    $scope.scopeModel.selectedTemplateConfig = undefined;
                };
                $scope.scopeModel.onAnalyticTableSelectorChanged = function (selectedItem) {

                    if (selectedItem != undefined) {

                        var input = {
                            TableIds: [selectedItem.AnalyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                        };

                        loadDimensions(input).then(function () {

                            var recordFilterDirectivePayload = {};
                            recordFilterDirectivePayload.context = buildContext();

                            var setLoader = function (value) {
                                $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader, tableSelectorSelectionChanged);
                        });
                        if (tableSelectedPromiseDeffered != undefined) {
                            tableSelectedPromiseDeffered.resolve();
                        }

                    }

                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};


                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.tileExtendedSettings != undefined) {

                            settings = payload.tileExtendedSettings;

                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            tableSelectedPromiseDeffered = UtilsService.createPromiseDeferred();
                            promises.push(tableSelectedPromiseDeffered.promise);

                            var loadDirectivePromiseDeffered = UtilsService.createPromiseDeferred();
                            promises.push(loadDirectivePromiseDeffered.promise);

                            UtilsService.waitMultiplePromises([directiveReadyDeferred.promise, tableSelectedPromiseDeffered.promise]).then(function () {
                               
                                directiveReadyDeferred = undefined;
                                var widgetEntity = {
                                    ChartType: settings.Settings.ChartType,
                                    Dimensions: settings.Settings.Dimensions,
                                    Measures: settings.Settings.Measures,
                                    OrderType: settings.Settings.OrderType,
                                    RootDimensionsFromSearch: settings.Settings.RootDimensionsFromSearch,
                                    TopRecords: settings.Settings.TopRecords,
                                    Measure: settings.Settings.Measure
                                };
                                directivePayload = {
                                    tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined,
                                    widgetEntity: widgetEntity
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectivePromiseDeffered);
                            });
                        };
                    }

                    var loadPeriodSelectorPromise = loadPeriodSelector();
                    promises.push(loadPeriodSelectorPromise);

                    var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    tableSelectorReadyDeferred.promise.then(function () {
                        var payLoadTableSelector = {
                            selectedIds: settings != undefined ? settings.AnalyticTableId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoadTableSelector, loadTableSelectorPromiseDeferred);
                    });

                    promises.push(loadTableSelectorPromiseDeferred.promise);

                    var getWidgetsTemplateConfigsPromise = getWidgetsTemplateConfigs();
                    promises.push(getWidgetsTemplateConfigsPromise);

                    if (settings != undefined) {
                        var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise(settings.RecordFilter);
                        promises.push(recordFilterDirectiveLoadPromise);
                    }
                    

                    return UtilsService.waitMultiplePromises(promises).then(function () {

                        tableSelectedPromiseDeffered = undefined;
                    });


                };

                api.getData = function () {
                    var directiveSettings = directiveAPI.getData();
                    if (directiveSettings != undefined)
                        directiveSettings.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    return {
                        $type:'Vanrise.Analytic.MainExtensions.Widget.WidgetSettings,Vanrise.Analytic.MainExtensions',
                        ConfigId : $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId,
                        AnalyticTableId: $scope.scopeModel.selectedTable != undefined ? $scope.scopeModel.selectedTable.AnalyticTableId : undefined,
                        TimePeriod: periodSelectorAPI.getData(),
                        RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                        Settings: directiveSettings
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadDimensions(input) {

                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
            }

            function loadPeriodSelector() {
                var loadPeriodSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                periodSelectorReadyPromiseDeferred.promise.then(function () {
                    var periodSelectorPayload;
                    if (settings != undefined)
                        periodSelectorPayload = {
                            timePeriod: settings.TimePeriod
                        };
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, loadPeriodSelectorPromiseDeferred);
                });
                return loadPeriodSelectorPromiseDeferred.promise;
            }

            function getWidgetsTemplateConfigs() {
                return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                    if (selectorAPI != undefined)
                        selectorAPI.clearDataSource();
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                        if (settings != undefined && settings.Settings != undefined)
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.Settings.ConfigId, 'ExtensionConfigurationId');
                    
                        //else
                        //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                    }
                });
            }

            function getRecordFilterDirectiveLoadPromise(recordFilter) {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                if (tableSelectorSelectionChanged == undefined)
                    tableSelectorSelectionChanged = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([recordFilterDirectiveReadyDeferred.promise, tableSelectorSelectionChanged.promise]).then(function () {
                    tableSelectorSelectionChanged = undefined;

                    var input = {
                        TableIds: [$scope.scopeModel.selectedTable.AnalyticTableId],
                        ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                    };

                    loadDimensions(input).then(function (response) {

                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        if (recordFilter != undefined) {
                            recordFilterDirectivePayload.FilterGroup = recordFilter;
                        }

                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });
                });

                return recordFilterDirectiveLoadDeferred.promise;
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        var fields = [];
                        if (dimensions != undefined) {
                            for (var i = 0; i < dimensions.length; i++) {
                                var dimension = dimensions[i];

                                fields.push({
                                    FieldName: dimension.Name,
                                    FieldTitle: dimension.Title,
                                    Type: dimension.Config.FieldType,
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            };
        }
    }

    app.directive('vrCommonWidgetsettingsDefinition', WidgetSettingsDefinition);

})(app);