(function (app) {

    'use strict';

    AnalyticreportRealtimeDefaultsearch.$inject = ["UtilsService", 'VRUIUtilsService', 'VRLocalizationService', 'VR_Analytic_AdvancedFilterFieldsRelationType', 'VR_Analytic_AdvancedFilterMeasuresRelationType'];

    function AnalyticreportRealtimeDefaultsearch(UtilsService, VRUIUtilsService, VRLocalizationService, VR_Analytic_AdvancedFilterFieldsRelationType, VR_Analytic_AdvancedFilterMeasuresRelationType) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var defaultRealTimeReportSearch = new DefaultRealTimeReportSearch($scope, ctrl, $attrs);
                defaultRealTimeReportSearch.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/RealTime/SearchSettings/Templates/DefaultRealTimeReportSearchTemplate.html"

        };

        function DefaultRealTimeReportSearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var filterDimensionSelectorAPI;
            var filterDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var advancedFilterDimensionSelectorAPI;
            var advancedFilterDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var advancedFilterMeasureSelectorAPI;
            var advancedFilterMeasureReadyDeferred = UtilsService.createPromiseDeferred();

            var localizationTextResourceSelectorAPI;
            var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.timeInterval = 15;
                $scope.scopeModel.filterDimensions = [];
                $scope.scopeModel.showAdvancedFilterFields = false;
                $scope.scopeModel.showAdvancedFilterMeasures = false;

                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.advancedFilterFieldsRelationTypeDS = UtilsService.getArrayEnum(VR_Analytic_AdvancedFilterFieldsRelationType);
                $scope.scopeModel.advancedFilterMeasuresRelationTypeDS = UtilsService.getArrayEnum(VR_Analytic_AdvancedFilterMeasuresRelationType);

                $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value, "value");

                $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                    filterDimensionSelectorAPI = api;
                    filterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                    localizationTextResourceSelectorAPI = api;
                    localizationTextResourceSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterDimensionSelectorDirectiveReady = function (api) {
                    advancedFilterDimensionSelectorAPI = api;
                    advancedFilterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterMeasureSelectorDirectiveReady = function (api) {
                    advancedFilterMeasureSelectorAPI = api;
                    advancedFilterMeasureReadyDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterFieldsRelationTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined)
                        return;

                    if (selectedItem.value == VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value) {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterFields = false;
                            $scope.scopeModel.selectedAdvancedFilterDimensions = [];
                        });
                    }
                    else {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterFields = true;
                        });
                    }
                };

                $scope.scopeModel.onAdvancedFilterMeasuresRelationTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined) {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterMeasures = false;
                            $scope.scopeModel.selectedAdvancedFilterMeasures = [];
                        });
                        return;
                    }

                    if (selectedItem.value == VR_Analytic_AdvancedFilterMeasuresRelationType.AllMeasures.value) {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterMeasures = false;
                            $scope.scopeModel.selectedAdvancedFilterMeasures = [];
                        });
                    }
                    else {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterMeasures = true;
                        });
                    }
                };

                $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRequired: false,
                        TitleResourceKey: dimension.TitleResourceKey
                    };
                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isDimensionTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.filterDimensions.push(dataItem);
                };

                $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeFilterDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFilterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedFilterDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                defineAPI();
            }


            function addSelectedFilterDimension(gridDimension) {

                var textResourcePayload;

                var dataItem = {};
                if (gridDimension.payload != undefined) {
                    dataItem.Name = gridDimension.payload.DimensionName;
                    dataItem.Title = gridDimension.payload.Title;
                    dataItem.IsRequired = gridDimension.IsRequired;
                    dataItem.oldTitleResourceKey = gridDimension.payload.TitleResourceKey;
                    textResourcePayload = { selectedValue: gridDimension.payload.TitleResourceKey };

                }

                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    gridDimension.textResourceReadyPromiseDeferred.resolve();
                };

                gridDimension.textResourceReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridDimension.textResourceLoadPromiseDeferred);
                    });

                $scope.scopeModel.filterDimensions.push(dataItem);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;

                        var selectedFilterIds;
                        var selectedAdvancedFilterFieldIds;
                        var selectedAdvancedFilterMeasureIds;

                        if (payload.searchSettings != undefined) {
                            $scope.scopeModel.timeInterval = payload.searchSettings.TimeIntervalInMin;

                            if (payload.searchSettings.Filters != undefined && payload.searchSettings.Filters.length > 0) {
                                selectedFilterIds = [];
                                for (var i = 0; i < payload.searchSettings.Filters.length; i++) {
                                    var filterDimension = payload.searchSettings.Filters[i];

                                    var dimensionGridField = {
                                        payload: filterDimension,
                                        textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    selectedFilterIds.push(filterDimension.DimensionName);

                                    if ($scope.scopeModel.isLocalizationEnabled)
                                        promises.push(dimensionGridField.textResourceLoadPromiseDeferred.promise);
                                    addSelectedFilterDimension(dimensionGridField);
                                }
                            }

                            if (payload.searchSettings.AdvancedFilters != undefined) {
                                var advancedFilters = payload.searchSettings.AdvancedFilters;
                                $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, advancedFilters.FieldsRelationType, "value");
                                $scope.scopeModel.selectedAdvancedFilterMeasuresRelationType = UtilsService.getItemByVal($scope.scopeModel.advancedFilterMeasuresRelationTypeDS, advancedFilters.MeasuresRelationType, "value");

                                if ($scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value == VR_Analytic_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                                    selectedAdvancedFilterFieldIds = [];
                                    for (var i = 0; i < advancedFilters.AvailableFields.length; i++) {
                                        var advancedFilterDimension = advancedFilters.AvailableFields[i];
                                        selectedAdvancedFilterFieldIds.push(advancedFilterDimension.FieldName);
                                    }
                                }

                                if ($scope.scopeModel.selectedAdvancedFilterMeasuresRelationType != undefined && $scope.scopeModel.selectedAdvancedFilterMeasuresRelationType.value == VR_Analytic_AdvancedFilterMeasuresRelationType.SpecificMeasures.value) {
                                    selectedAdvancedFilterMeasureIds = [];
                                    for (var i = 0; i < advancedFilters.AvailableMeasures.length; i++) {
                                        var advancedFilterMeasure = advancedFilters.AvailableMeasures[i];
                                        selectedAdvancedFilterMeasureIds.push(advancedFilterMeasure.FieldName);
                                    }
                                }
                            }
                        }

                        var loadFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        filterDimensionReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds, HideIsRequiredFromParent: true },
                                selectedIds: selectedFilterIds
                            };

                            VRUIUtilsService.callDirectiveLoad(filterDimensionSelectorAPI, payloadFilterDirective, loadFilterDirectivePromiseDeferred);
                        });
                        promises.push(loadFilterDirectivePromiseDeferred.promise);

                        var loadAdvancedFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        advancedFilterDimensionReadyDeferred.promise.then(function () {
                            var payloadAdvancedFilterDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedAdvancedFilterFieldIds
                            };
                            VRUIUtilsService.callDirectiveLoad(advancedFilterDimensionSelectorAPI, payloadAdvancedFilterDirective, loadAdvancedFilterDirectivePromiseDeferred);
                        });
                        promises.push(loadAdvancedFilterDirectivePromiseDeferred.promise);

                        var advancedFilterMeasureLoadDeferred = UtilsService.createPromiseDeferred();
                        advancedFilterMeasureReadyDeferred.promise.then(function () {
                            var payloadAdvancedFilterMeasureDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedAdvancedFilterMeasureIds
                            };
                            VRUIUtilsService.callDirectiveLoad(advancedFilterMeasureSelectorAPI, payloadAdvancedFilterMeasureDirective, advancedFilterMeasureLoadDeferred);
                        });
                        promises.push(advancedFilterMeasureLoadDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var filterDimensions;
                    if ($scope.scopeModel.filterDimensions != undefined && $scope.scopeModel.filterDimensions.length > 0) {
                        filterDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.filterDimensions.length; i++) {
                            var filterDimension = $scope.scopeModel.filterDimensions[i];
                            filterDimensions.push({
                                DimensionName: filterDimension.Name,
                                Title: filterDimension.Title,
                                IsRequired: filterDimension.IsRequired,
                                TitleResourceKey: filterDimension.textResourceSeletorAPI != undefined ? filterDimension.textResourceSeletorAPI.getSelectedValues() : filterDimension.oldTitleResourceKey
                            });
                        }
                    }

                    var advancedFilters = {};
                    advancedFilters.FieldsRelationType = $scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value;
                    advancedFilters.MeasuresRelationType = $scope.scopeModel.selectedAdvancedFilterMeasuresRelationType != undefined ? $scope.scopeModel.selectedAdvancedFilterMeasuresRelationType.value : undefined;

                    if ($scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value == VR_Analytic_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                        advancedFilters.AvailableFields = [];
                        for (var i = 0; i < $scope.scopeModel.selectedAdvancedFilterDimensions.length; i++) {
                            var advancedFilterField = $scope.scopeModel.selectedAdvancedFilterDimensions[i];
                            advancedFilters.AvailableFields.push({
                                FieldName: advancedFilterField.Name
                            });
                        }
                    }

                    if ($scope.scopeModel.selectedAdvancedFilterMeasuresRelationType != undefined && $scope.scopeModel.selectedAdvancedFilterMeasuresRelationType.value == VR_Analytic_AdvancedFilterMeasuresRelationType.SpecificMeasures.value) {
                        advancedFilters.AvailableMeasures = [];
                        for (var i = 0; i < $scope.scopeModel.selectedAdvancedFilterMeasures.length; i++) {
                            var advancedFilterMeasure = $scope.scopeModel.selectedAdvancedFilterMeasures[i];
                            advancedFilters.AvailableMeasures.push({
                                FieldName: advancedFilterMeasure.Name,
                            });
                        }
                    }

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.RealTimeReport.SearchSettings.DefaultRealTimeReportSearch, Vanrise.Analytic.MainExtensions ",
                        Filters: filterDimensions,
                        AdvancedFilters: advancedFilters,
                        TimeIntervalInMin: $scope.scopeModel.timeInterval
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticAnalyticreportRealtimeSearchsettingsDefaultsearch', AnalyticreportRealtimeDefaultsearch);

})(app);