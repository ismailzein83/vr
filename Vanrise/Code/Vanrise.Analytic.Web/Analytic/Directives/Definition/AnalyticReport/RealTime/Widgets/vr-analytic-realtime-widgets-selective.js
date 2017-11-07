(function (app) {

    'use strict';

    AnalyticRealtimeWidgetsSelective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService', 'ColumnWidthEnum', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_AnalyticTypeEnum'];

    function AnalyticRealtimeWidgetsSelective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService, ColumnWidthEnum, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticTypeEnum) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RealTime/Widgets/Templates/RealTimeWidgetsSelectiveTemplate.html",
            //template: function (element, attrs) {
            //    return getTamplate(attrs);
            //}
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Widgets'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }

            var template =
                '<vr-row>'
               + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-textbox label="Title" value="scopeModel.widgetTitle" isrequired="true"></vr-textbox>'
              + ' </vr-columns>'
               + '  <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + '  <vr-select datasource="scopeModel.columnWidth" datatextfield="description" datavaluefield="value" hideremoveicon label="Row Space" selectedvalues="scopeModel.selectedColumnWidth" isrequired hidefilterbox></vr-select>'
                + ' </vr-columns>'
               + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-analytic-table-selector on-ready="scopeModel.onTableSelectorDirectiveReady" isrequired="true" selectedvalues="scopeModel.selectedTable" hideremoveicon onselectitem="scopeModel.onSelectionTableChanged"></vr-analytic-table-selector>'
              + ' </vr-columns>'

              + '<vr-columns colnum="{{searchSettingsCtrl.normalColNum}}" ng-if="scopeModel.selectedTable !=undefined">'
              + ' <vr-select on-ready="scopeModel.onSelectorReady"'
              + ' datasource="scopeModel.templateConfigs"'
              + ' selectedvalues="scopeModel.selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="true"'
              + 'hideremoveicon>'
          + '</vr-select>'
           + ' </vr-columns>'

                      + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-switch label="Show Title" value="scopeModel.showTitle"></vr-switch>'
                + ' </vr-columns>'

           + '</vr-row>'
              + '<vr-directivewrapper directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{searchSettingsCtrl.normalColNum}}" isrequired="searchSettingsCtrl.isrequired" customvalidate="searchSettingsCtrl.customvalidate" type="searchSettingsCtrl.type"></vr-directivewrapper>';
            return template;
        }

        function Widgets($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var widgetEntity;
            var tableIds;
            var dimensions;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableSelectorSelectionChanged;

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
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {
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

                $scope.scopeModel.onTableSelectorSelectionChanged = function (selectedItem) {
                    //$scope.scopeModel.selectedTemplateConfig = undefined;

                    if (selectedItem != undefined) {

                        if (tableSelectorSelectionChanged != undefined) {
                            tableSelectorSelectionChanged.resolve();
                        }
                        else {
                            var input = {
                                TableIds: [selectedItem.AnalyticTableId],
                                ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value
                            };

                            loadDimensions(input).then(function () {

                                var recordFilterDirectivePayload = {};
                                recordFilterDirectivePayload.context = buildContext();

                                var setLoader = function (value) {
                                    $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader);
                            });
                        }
                    }
                };

                defineColumnWidth();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var isEditMode;

                    if (payload != undefined) {
                        tableIds = payload.tableIds;

                        if (payload.widgetEntity != undefined) {
                            isEditMode = true;

                            widgetEntity = payload.widgetEntity;
                            $scope.scopeModel.widgetTitle = widgetEntity.WidgetTitle;
                            $scope.scopeModel.selectedColumnWidth = UtilsService.getItemByVal($scope.scopeModel.columnWidth, widgetEntity.ColumnWidth, "value");
                            $scope.scopeModel.showTitle = widgetEntity.ShowTitle;
                        }

                        //Loading Table Selector 
                        var tableSelectorLoadPromise = getTableSelectorLoadPromise();
                        promises.push(tableSelectorLoadPromise);

                        //Loading Widgets Selector
                        var widgetsTemplateConfigsPromise = getWidgetsTemplateConfigs();
                        promises.push(widgetsTemplateConfigsPromise);

                        if (isEditMode) {
                            //Loading DirectiveWrapper
                            var directiveWrapperLoadPromise = getDirectiveWrapperLoadPromise();
                            promises.push(directiveWrapperLoadPromise);

                            //Loading RecordFilter Directive
                            var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                            promises.push(recordFilterDirectiveLoadPromise);
                        }

                        function getTableSelectorLoadPromise() {
                            var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                            tableSelectorReadyDeferred.promise.then(function () {

                                var payLoadTableSelector = {
                                    filter: { OnlySelectedIds: tableIds },
                                    selectedIds: widgetEntity != undefined ? widgetEntity.AnalyticTableId : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoadTableSelector, loadTableSelectorPromiseDeferred);
                            });

                            return loadTableSelectorPromiseDeferred.promise;
                        }
                        function getWidgetsTemplateConfigs() {
                            return VR_Analytic_AnalyticConfigurationAPIService.GetRealTimeWidgetsTemplateConfigs().then(function (response) {
                                if (selectorAPI != undefined) {
                                    selectorAPI.clearDataSource();
                                }

                                if (response != null) {
                                    for (var i = 0; i < response.length; i++) {
                                        $scope.scopeModel.templateConfigs.push(response[i]);
                                    }

                                    if (widgetEntity != undefined) {
                                        $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, widgetEntity.ConfigId, 'ExtensionConfigurationId');
                                    }
                                }
                            });
                        }
                        function getDirectiveWrapperLoadPromise() {
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;

                                var payloadDirective = {
                                    tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined,
                                    widgetEntity: widgetEntity
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });

                            return loadDirectivePromiseDeferred.promise;
                        }
                        function getRecordFilterDirectiveLoadPromise() {
                            var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                            if (tableSelectorSelectionChanged == undefined)
                                tableSelectorSelectionChanged = UtilsService.createPromiseDeferred();

                            UtilsService.waitMultiplePromises([recordFilterDirectiveReadyDeferred.promise, tableSelectorSelectionChanged.promise]).then(function () {
                                tableSelectorSelectionChanged = undefined;

                                var input = {
                                    TableIds: [$scope.scopeModel.selectedTable.AnalyticTableId],
                                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                                };

                                loadDimensions(input).then(function () {

                                    var recordFilterDirectivePayload = {};
                                    recordFilterDirectivePayload.context = buildContext();
                                    if (widgetEntity != undefined && widgetEntity.RecordFilter != undefined) {
                                        recordFilterDirectivePayload.FilterGroup = widgetEntity.RecordFilter;
                                    }

                                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                                });
                            });

                            return recordFilterDirectiveLoadDeferred.promise;
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                            data.AnalyticTableId = $scope.scopeModel.selectedTable != undefined ? $scope.scopeModel.selectedTable.AnalyticTableId : undefined,
                            data.WidgetTitle = $scope.scopeModel.widgetTitle;
                            data.ColumnWidth = $scope.scopeModel.selectedColumnWidth.value;
                            data.ShowTitle = $scope.scopeModel.showTitle;
                            data.RecordFilter = recordFilterDirectiveAPI.getData().filterObj;
                        }
                    }
                    return data;
                };

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
                };
                return context;
            };
        }
    }

    app.directive('vrAnalyticRealtimeWidgetsSelective', AnalyticRealtimeWidgetsSelective);

})(app);