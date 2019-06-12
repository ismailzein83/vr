﻿(function (app) {

    'use strict';

    ReportsearchsettingsGenericsearch.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AdvancedFilterFieldsRelationType','VRLocalizationService'];

    function ReportsearchsettingsGenericsearch(UtilsService, VRUIUtilsService, VR_Analytic_AdvancedFilterFieldsRelationType, VRLocalizationService) {
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
                var ctor = new GenericSearchCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/SearchSettings/Templates/GenericSearchTemplate.html"
        };

        function GenericSearchCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var groupingDimensionSelectorAPI;
            var groupingDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDimensionSelectorAPI;
            var filterDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var advancedFilterDimensionSelectorAPI;
            var advancedFilterDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.analyticTables = [];
                $scope.scopeModel.groupingDimensions = [];
                $scope.scopeModel.filterDimensions = [];
                $scope.scopeModel.showAdvancedFilterFields = false;
                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                //$scope.scopeModel.advancedFilterFields = [];
                $scope.scopeModel.advancedFilterFieldsRelationTypeDS = UtilsService.getArrayEnum(VR_Analytic_AdvancedFilterFieldsRelationType);
                $scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues =
                    UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value, "value");

                $scope.scopeModel.onGroupingDimensionSelectorDirectiveReady = function (api) {
                    groupingDimensionSelectorAPI = api;
                    groupingDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                    filterDimensionSelectorAPI = api;
                    filterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterDimensionSelectorDirectiveReady = function (api) {
                    advancedFilterDimensionSelectorAPI = api;
                    advancedFilterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterFieldsRelationTypeSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        if (selectedItem.value == VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value) {
                            setTimeout(function () {
                                $scope.scopeModel.showAdvancedFilterFields = false;
                            });
                        }
                        else {
                            setTimeout(function () {
                                $scope.scopeModel.showAdvancedFilterFields = true;
                            });
                        }
                    }
                };

                $scope.scopeModel.onSelectGroupingDimensionItem = function (dimension) {
                    var dataItem = {
                        Name: dimension.Name,
                        Title: dimension.Title,
                        IsSelected: false,
                        TitleResourceKey: dimension.TitleResourceKey
                    };
                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isDimensionTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.groupingDimensions.push(dataItem);
                };
                $scope.scopeModel.onDeselectGroupingDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                    var dataItem = {
                        Name: dimension.Name,
                        Title: dimension.Title,
                        IsRequired: false,
                        TitleResourceKey: dimension.TitleResourceKey
                    };
                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isFilterDimensionTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.filterDimensions.push(dataItem);
                };
                $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                //$scope.scopeModel.onSelectAdvancedFilterDimensionItem = function (dimension) {
                //    var dataItem = {
                //        Name: dimension.Name,
                //        Title: dimension.Title
                //    };
                //    $scope.scopeModel.advancedFilterFields.push(dataItem);
                //};
                //$scope.scopeModel.onDeselectAdvancedFilterDimensionItem = function (dataItem) {
                //    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.advancedFilterFields, dataItem.Name, 'Name');
                //    $scope.scopeModel.advancedFilterFields.splice(datasourceIndex, 1);
                //};

                $scope.scopeModel.removeGroupingDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeFilterDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFilterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedFilterDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeAdvancedFilterField = function (dataItem) {
                    var advancedFilterDimensionIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedAdvancedFilterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedAdvancedFilterDimensions.splice(advancedFilterDimensionIndex, 1);
                    //var advancedFilterFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.advancedFilterFields, dataItem.Name, 'Name');
                    //$scope.scopeModel.advancedFilterFields.splice(advancedFilterFieldIndex, 1);
                };

                $scope.scopeModel.isValidGroupingDimensions = function () {

                    if ($scope.scopeModel.groupingDimensions.length > 0 || !$scope.scopeModel.isRequiredGroupingDimensions)
                        return null;
                    return "At least one dimension should be selected.";
                };
                //$scope.scopeModel.isValidAdvancedFilterFields = function () {
                //    if ($scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues == undefined)
                //        return null;
                //    if ($scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues.value == VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value) {
                //        return null;
                //    }
                //    else {
                //        if ($scope.scopeModel.advancedFilterFields.length > 0)
                //            return null;
                //        return "At least one dimension should be selected.";
                //    }
                //};

                defineAPI();
            }
            function addSelectedDimension(gridDimension) {

                var textResourcePayload;

                var dataItem = {};
                if (gridDimension.payload != undefined) {
                    dataItem.Name = gridDimension.payload.DimensionName;
                    dataItem.Title = gridDimension.payload.Title != undefined ? gridDimension.payload.Title : gridDimension.payload.DimensionName;
                    dataItem.IsSelected = gridDimension.payload.IsRequired;
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

                $scope.scopeModel.groupingDimensions.push(dataItem);

            }
            function addSelectedFilterDimension(filterDimension) {

                var textResourcePayload;

                var dataItem = {};
                if (filterDimension.payload != undefined) {
                    dataItem.Name = filterDimension.payload.DimensionName;
                    dataItem.Title = filterDimension.payload.Title != undefined ? filterDimension.payload.Title : filterDimension.payload.DimensionName;
                    dataItem.IsRequired = filterDimension.payload.IsSelected;
                    textResourcePayload = { selectedValue: filterDimension.payload.TitleResourceKey };

                }
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    filterDimension.textResourceReadyPromiseDeferred.resolve();
                };

                filterDimension.textResourceReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, filterDimension.textResourceLoadPromiseDeferred);
                    });

                $scope.scopeModel.filterDimensions.push(dataItem);

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = {};
                    var promises = [];

                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        $scope.scopeModel.analyticTables = tableIds;

                        var selectedGroupingIds;
                        var selectedFilterIds;
                        var selectedAdvancedFilterFieldIds;

                        if (payload.searchSettings != undefined) {
                            $scope.scopeModel.isRequiredGroupingDimensions = payload.searchSettings.IsRequiredGroupingDimensions;
                            $scope.scopeModel.showCurrency = payload.searchSettings.ShowCurrency;

                            var groupingDimensions = payload.searchSettings.GroupingDimensions;
                            if (groupingDimensions != undefined && groupingDimensions.length > 0) {
                                selectedGroupingIds = [];
                                for (var i = 0; i < groupingDimensions.length; i++) {
                                    var groupingDimension = groupingDimensions[i];
                                    selectedGroupingIds.push(groupingDimension.DimensionName);
                                }
                            }
                            var filters =payload.searchSettings.Filters;
                            if (filters != undefined && filters.length > 0) {
                                selectedFilterIds = [];
                                for (var i = 0; i < filters.length; i++) {
                                    var filterDimension = filters[i];
                                    selectedFilterIds.push(filterDimension.DimensionName);
                                   
                                }
                            }
                            if (payload.searchSettings != undefined)
                                $scope.scopeModel.showLegend = payload.searchSettings.ShowLegend;
                            if (payload.searchSettings.AdvancedFilters != undefined) {
                                $scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues =
                                    UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, payload.searchSettings.AdvancedFilters.FieldsRelationType, "value");

                                if ($scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues.value == VR_Analytic_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                                    selectedAdvancedFilterFieldIds = [];
                                    for (var i = 0; i < payload.searchSettings.AdvancedFilters.AvailableFields.length; i++) {
                                        var advancedFilterDimension = payload.searchSettings.AdvancedFilters.AvailableFields[i];
                                        selectedAdvancedFilterFieldIds.push(advancedFilterDimension.FieldName);
                                        //var dataItem = {
                                        //    Name: advancedFilterDimension.FieldName,
                                        //    Title: advancedFilterDimension.FieldTitle
                                        //};
                                        //$scope.scopeModel.advancedFilterFields.push(dataItem);
                                    }
                                }
                            }
                        }

                        var loadGroupingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        groupingDimensionReadyDeferred.promise.then(function () {
                            var payloadGroupingDirective = {
                                filter: { TableIds: tableIds, HideIsRequiredFromParent: true },
                                selectedIds: selectedGroupingIds
                            };
                            VRUIUtilsService.callDirectiveLoad(groupingDimensionSelectorAPI, payloadGroupingDirective, loadGroupingDirectivePromiseDeferred);
                        });
                        promises.push(loadGroupingDirectivePromiseDeferred.promise);

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
                       


                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadTableSelectorPromiseDeferred.promise);
                        tableSelectorReadyDeferred.promise.then(function () {
                            var tableSelectorpayLoad = {
                                filter: {
                                    OnlySelectedIds: tableIds
                                }
                            };
                            if (payload.searchSettings != undefined && payload.searchSettings.Legends != undefined && payload.searchSettings.Legends.length > 0) {
                                tableSelectorpayLoad.selectedIds = payload.searchSettings.Legends[0].AnalyticTableId;
                            }
                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, tableSelectorpayLoad, loadTableSelectorPromiseDeferred);
                        });

                        rootPromiseNode.promises = promises;
                        rootPromiseNode.getChildNode = function () {
                            var childPromises = [];
                            if ($scope.scopeModel.selectedGroupingDimensions != undefined && $scope.scopeModel.selectedGroupingDimensions.length > 0) {
                                for (var i = 0; i < groupingDimensions.length; i++) {
                                    var groupingDimension = groupingDimensions[i];
                                    if (UtilsService.getItemByVal($scope.scopeModel.selectedGroupingDimensions, groupingDimension.DimensionName, "Name")) {
                                        var dimensionGridField = {
                                            payload: groupingDimension,
                                            textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        if ($scope.scopeModel.isLocalizationEnabled)
                                            childPromises.push(dimensionGridField.textResourceLoadPromiseDeferred.promise);
                                        addSelectedDimension(dimensionGridField);
                                    }
                                }
                            }
                            if ($scope.scopeModel.selectedFilterDimensions != undefined && $scope.scopeModel.selectedFilterDimensions.length > 0) {
                                for (var i = 0; i < filters.length; i++) {
                                    var filterDimension = filters[i];
                                    if (UtilsService.getItemByVal($scope.scopeModel.selectedFilterDimensions, filterDimension.DimensionName, "Name")) {
                                        var filterDimensionField = {
                                            payload: filterDimension,
                                            textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        if ($scope.scopeModel.isLocalizationEnabled)
                                            childPromises.push(filterDimensionField.textResourceLoadPromiseDeferred.promise);
                                        addSelectedFilterDimension(filterDimensionField);
                                    }
                                }
                            }
                            return { promises: childPromises };
                        };

                        return UtilsService.waitPromiseNode(rootPromiseNode);
                    };
                };
                api.getData = function getData() {

                    var groupingDimensions;
                    if ($scope.scopeModel.groupingDimensions != undefined && $scope.scopeModel.groupingDimensions.length > 0) {
                        groupingDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.groupingDimensions.length; i++) {
                            var groupingDimension = $scope.scopeModel.groupingDimensions[i];
                            groupingDimensions.push({
                                DimensionName: groupingDimension.Name,
                                IsSelected: groupingDimension.IsSelected,
                                Title: groupingDimension.Title,
                                TitleResourceKey: groupingDimension.textResourceSeletorAPI != undefined ? groupingDimension.textResourceSeletorAPI.getSelectedValues() : undefined
                            });
                        }
                    }

                    var filterDimensions;
                    if ($scope.scopeModel.filterDimensions != undefined && $scope.scopeModel.filterDimensions.length > 0) {
                        filterDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.filterDimensions.length; i++) {
                            var filterDimension = $scope.scopeModel.filterDimensions[i];
                            filterDimensions.push({
                                DimensionName: filterDimension.Name,
                                Title: filterDimension.Title,
                                IsRequired: filterDimension.IsRequired,
                                TitleResourceKey: filterDimension.textResourceSeletorAPI != undefined ? filterDimension.textResourceSeletorAPI.getSelectedValues() : undefined
                            });
                        }
                    }

                    var advancedFilters = {};
                    advancedFilters.FieldsRelationType = $scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues.value;
                    if ($scope.scopeModel.advancedFilterFieldsRelationTypeSelectedValues.value == VR_Analytic_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                        advancedFilters.AvailableFields = [];
                        for (var i = 0; i < $scope.scopeModel.selectedAdvancedFilterDimensions.length; i++) {
                            var advancedFilterField = $scope.scopeModel.selectedAdvancedFilterDimensions[i];
                            advancedFilters.AvailableFields.push({
                                FieldName: advancedFilterField.Name,
                                //FieldTitle: advancedFilterField.Title
                            });
                        }
                    }

                    var legends = [];
                    var selectedTableId = tableSelectorAPI.getSelectedIds();
                    if (selectedTableId != undefined) {
                        legends.push({
                            AnalyticTableId: selectedTableId
                        });
                    }
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.SearchSettings.GenericSearchSettings, Vanrise.Analytic.MainExtensions ",
                        IsRequiredGroupingDimensions: $scope.scopeModel.isRequiredGroupingDimensions,
                        ShowCurrency: $scope.scopeModel.showCurrency,
                        GroupingDimensions: groupingDimensions,
                        Filters: filterDimensions,
                        AdvancedFilters: advancedFilters,
                        ShowLegend: $scope.scopeModel.showLegend,
                        Legends: legends
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }
        }
    }
    app.directive('vrAnalyticReportsearchsettingsGenericsearch', ReportsearchsettingsGenericsearch);

})(app);