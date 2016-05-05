(function (appControllers) {
    'use strict';

    genericAnalyticReportController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_Sec_ViewAPIService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function genericAnalyticReportController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_Sec_ViewAPIService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        var viewId;
        var viewEntity;
        var fieldTypes = [];
        var dimensions = [];
        var measures = [];
        loadParameters();
        defineScope();
        load();



        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.widgets = [];
            $scope.scopeModel.filters = [];
            $scope.scopeModel.fromdate = "01/01/2015";
            $scope.scopeModel.todate = new Date();
            $scope.scopeModel.groupingDimentions = [];
            $scope.scopeModel.selectedGroupingDimentions = [];
            $scope.scopeModel.isGroupingRequired = false;
            $scope.search = function () {
                if ($scope.scopeModel.widgets.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.widgets.length ; i++) {
                        var widget = $scope.scopeModel.widgets[i];
                        var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                        var payload = getQuery(widget.settings);
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, widget.directiveAPI, payload, setLoader);
                    }
                }
            };


        }

        function load() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([getView, getWidgetsTemplateConfigs, getFieldTypeConfigs]).then(function () {
                UtilsService.waitMultipleAsyncOperations([loadDimensions, loadMeasures]).then(function () {
                    loadAllControls()
                }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });

            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadWidgets, loadFilters ]).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });


            }
            function getWidgetsTemplateConfigs() {
                return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                    }
                });
            }

            function getFieldTypeConfigs() {
                return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                    fieldTypes.length = 0;
                    for (var i = 0; i < response.length; i++) {
                        fieldTypes.push(response[i]);
                    }
                });
            }

            function loadMeasures() {
                var input = {
                    TableIds: viewEntity.Settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                }
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    measures = response;
                });
            }
            function loadDimensions() {
                var input = {
                    TableIds: viewEntity.Settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                }
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
            }
            function loadFilters() {
                var filterPromises = [];
                $scope.scopeModel.isGroupingRequired = viewEntity.Settings.SearchSettings.IsRequiredGroupingDimensions;
                if (viewEntity.Settings.SearchSettings.Filters != undefined) {
                    for (var i = 0; i < viewEntity.Settings.SearchSettings.Filters.length; i++) {
                        var filterConfiguration = viewEntity.Settings.SearchSettings.Filters[i];
                        var filter = getFilter(filterConfiguration);
                        if (filter != undefined) {
                            filterPromises.push(filter.directiveLoadDeferred.promise);
                            $scope.scopeModel.filters.push(filter);
                        }
                    }
                }

                if (viewEntity.Settings.SearchSettings.GroupingDimensions != undefined) {
                    for (var i = 0; i < viewEntity.Settings.SearchSettings.GroupingDimensions.length; i++) {
                        var groupingDimention = viewEntity.Settings.SearchSettings.GroupingDimensions[i];
                        $scope.scopeModel.groupingDimentions.push(groupingDimention);
                        if (groupingDimention.IsSelected) {
                            $scope.scopeModel.selectedGroupingDimentions.push(groupingDimention);
                        }
                    }
                }

                return UtilsService.waitMultiplePromises(filterPromises);

                function getFilter(filterConfiguration) {
                     var dimension = UtilsService.getItemByVal(dimensions, filterConfiguration.DimensionName, 'Name');
                    var filter;
                    var filterEditor = UtilsService.getItemByVal(fieldTypes, filterConfiguration.FieldType.ConfigId, 'DataRecordFieldTypeConfigId').FilterEditor;

                    if (filterEditor == null) return filter;

                    filter = {};
                    filter.dimesnionName = filterConfiguration.DimensionName;
                    filter.isRequired = filterConfiguration.IsRequired;
                    filter.directiveEditor = filterEditor;
                    filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    filter.onDirectiveReady = function (api) {
                        filter.directiveAPI = api;
                        var directivePayload = {
                            fieldTitle: filterConfiguration.Title,
                            fieldType:dimension !=undefined?dimension.Config.FieldType:undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                    };

                    return filter;
                }
            }

            function loadWidgets() {
                var promises = [];
                for (var i = 0; i < viewEntity.Settings.Widgets.length; i++) {
                    var widgetItem = {
                        payload: viewEntity.Settings.Widgets[i],
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };
                    promises.push(widgetItem.loadPromiseDeferred.promise);
                    addWidget(widgetItem);
                }

                function addWidget(widgetItem) {
                    var matchItem = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, widgetItem.payload.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;
                    var widget = {
                        id: $scope.scopeModel.widgets.length + 1,
                        configId: matchItem.ExtensionConfigurationId,
                        runtimeEditor: matchItem.RuntimeEditor,
                        name: widgetItem.WidgetTitle,
                        settings: widgetItem.payload
                    };
                    var dataItemPayload = getQuery(widgetItem.payload);

                    widget.onDirectiveReady = function (api) {

                        widget.directiveAPI = api;
                        widgetItem.readyPromiseDeferred.resolve();
                    };

                    widgetItem.readyPromiseDeferred.promise
                        .then(function () {

                            // widget.directiveAPI.loadGrid(dataItemPayload);
                            //  widgetItem.loadPromiseDeferred.resolve();

                            VRUIUtilsService.callDirectiveLoad(widget.directiveAPI, dataItemPayload, widgetItem.loadPromiseDeferred);
                        });
                    $scope.scopeModel.widgets.push(widget);
                }



            }
        }

        function getQuery(widgetPayload) {
            var dimensionFilters = [];
            if ($scope.scopeModel.filters != undefined) {
                for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                    var filter = $scope.scopeModel.filters[i];
                    if (filter.directiveAPI.getData() != undefined) {
                        dimensionFilters.push({
                            Dimension: filter.dimesnionName,
                            FilterValues: filter.directiveAPI.getValuesAsArray()
                        })
                    }

                }
            }


            var groupingDimensions = [];

            if ($scope.scopeModel.selectedGroupingDimentions != undefined && $scope.scopeModel.selectedGroupingDimentions.length > 0) {
                for (var i = 0; i < $scope.scopeModel.selectedGroupingDimentions.length; i++) {
                    groupingDimensions.push({ DimensionName: $scope.scopeModel.selectedGroupingDimentions[i].DimensionName });

                }
            } else {
                if (viewEntity.Settings.SearchSettings.GroupingDimensions != undefined) {
                    for (var i = 0; i < viewEntity.Settings.SearchSettings.GroupingDimensions.length; i++) {
                        var groupDimension = viewEntity.Settings.SearchSettings.GroupingDimensions[i];
                        if (groupDimension.IsSelected) {
                            groupingDimensions.push({ DimensionName: groupDimension.DimensionName });
                        }
                    }
                }

            }
            var query = {
                Settings: widgetPayload,
                DimensionFilters: dimensionFilters,
                GroupingDimensions: groupingDimensions,
                TableId: widgetPayload.AnalyticTableId,
                FromTime: $scope.scopeModel.fromdate,
                ToTime: $scope.scopeModel.todate
            };
            return query;
        }

        function getView() {
            return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                viewEntity = viewEntityObj;
            });
        }
    }

    appControllers.controller('VR_Analytic_GenericAnalyticReportController', genericAnalyticReportController);

})(appControllers);
