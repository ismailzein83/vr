(function (app) {

    'use strict';

    AnalyticWidgetsSelective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'VR_Analytic_AnalyticTypeEnum', 'UtilsService', 'VRUIUtilsService', 'ColumnWidthEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function AnalyticWidgetsSelective(VR_Analytic_AnalyticConfigurationAPIService, VR_Analytic_AnalyticTypeEnum, UtilsService, VRUIUtilsService, ColumnWidthEnum, VR_Analytic_AnalyticItemConfigAPIService) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/History/Widgets/Templates/WidgetsSelectiveTemplate.html"
        };

        function Widgets($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var tableIds;
            var dimensions;
            var widgetEntity;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableSelectorSelectionChanged;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectionTableChanged = function () {
                    $scope.scopeModel.selectedTemplateConfig = undefined;
                };

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;

                };
                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
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

                $scope.scopeModel.onAnalyticTableSelectorChanged = function (selectedItem) {

                    if (selectedItem != undefined) {

                        var input = {
                            TableIds: [selectedItem.AnalyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                        };

                        loadDimensions(input).then(function (response) {

                            var setLoader = function (value) {
                                $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                            };
                            var recordFilterDirectivePayload = {};
                            recordFilterDirectivePayload.context = buildContext();

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader, tableSelectorSelectionChanged);
                        });
                    }
                }

                defineColumnWidth();
                defineAPI();
            }
            function defineAPI() {
                var api = {};


                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined) {
                        tableIds = payload.tableIds;

                        if (payload.widgetEntity != undefined) {

                            widgetEntity = payload.widgetEntity;
                            $scope.scopeModel.widgetTitle = widgetEntity.WidgetTitle;
                            $scope.scopeModel.selectedColumnWidth = UtilsService.getItemByVal($scope.scopeModel.columnWidth, widgetEntity.ColumnWidth, "value");
                            $scope.scopeModel.showTitle = widgetEntity.ShowTitle;

                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = {
                                    tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined,
                                    widgetEntity: widgetEntity
                                };

                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }

                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        tableSelectorReadyDeferred.promise.then(function () {
                            var payLoadTableSelector = {
                                filter: { OnlySelectedIds: tableIds },
                                selectedIds: widgetEntity != undefined ? widgetEntity.AnalyticTableId : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoadTableSelector, loadTableSelectorPromiseDeferred);
                        });
                        promises.push(loadTableSelectorPromiseDeferred.promise);

                        var getWidgetsTemplateConfigsPromise = getWidgetsTemplateConfigs();
                        promises.push(getWidgetsTemplateConfigsPromise);

                        //Loading Record Filter Directive @EditMode only
                        if (widgetEntity != undefined) {
                            var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise(widgetEntity.RecordFilter);
                            promises.push(recordFilterDirectiveLoadPromise);
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function defineColumnWidth() {
                $scope.scopeModel.columnWidth = [];
                for (var td in ColumnWidthEnum)
                    $scope.scopeModel.columnWidth.push(ColumnWidthEnum[td]);
                $scope.scopeModel.selectedColumnWidth = ColumnWidthEnum.FullRow;
            }

            function getData() {
                var data;
                if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                    data = directiveAPI.getData();
                    if (data != undefined) {
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        data.AnalyticTableId = $scope.scopeModel.selectedTable != undefined ? $scope.scopeModel.selectedTable.AnalyticTableId : undefined;
                        data.WidgetTitle = $scope.scopeModel.widgetTitle;
                        data.ColumnWidth = $scope.scopeModel.selectedColumnWidth.value;
                        data.ShowTitle = $scope.scopeModel.showTitle;
                        data.RecordFilter = recordFilterDirectiveAPI.getData().filterObj
                    }
                }
                return data;
            }
            function getWidgetsTemplateConfigs() {
                return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                    if (selectorAPI != undefined)
                        selectorAPI.clearDataSource();
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                        if (widgetEntity != undefined)
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, widgetEntity.ConfigId, 'ExtensionConfigurationId');
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
            function loadDimensions(input) {

                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
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
                }
                return context;
            };
        }
    }

    app.directive('vrAnalyticWidgetsSelective', AnalyticWidgetsSelective);

})(app);