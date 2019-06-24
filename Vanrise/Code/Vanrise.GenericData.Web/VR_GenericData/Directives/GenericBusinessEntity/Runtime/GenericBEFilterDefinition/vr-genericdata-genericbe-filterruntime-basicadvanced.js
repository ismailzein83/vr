﻿(function (app) {

    'use strict';

	BasicAdvancedFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_RecordQueryLogicalOperatorEnum','VRLocalizationService'];

	function BasicAdvancedFilterRuntimeSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_RecordQueryLogicalOperatorEnum, VRLocalizationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/BasicAdvancedFilterRuntimeTemplate.html"
        };

        function BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;
            var isFromManagementScreen;

            function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();
                $scope.scopeModel.showBasicAdvancedTabs = false;

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.filters = [];

                    var promises = [];

                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        isFromManagementScreen = payload.isFromManagementScreen;

                        var settings = payload.settings;
                        var filterValues = payload.filterValues;
                        var genericContext = payload.genericContext;
                        var allFieldValuesByName = payload.allFieldValuesByName;

                        if (settings != undefined && settings.Filters != undefined && settings.Filters.length > 0) {
                            var firstFilterShowInBasic = settings.Filters[0].ShowInBasic;

                            for (var i = 0; i < settings.Filters.length; i++) {
                                var filter = settings.Filters[i];

                                if (!$scope.scopeModel.showBasicAdvancedTabs && firstFilterShowInBasic != filter.ShowInBasic) {
                                    $scope.scopeModel.showBasicAdvancedTabs = true;
                                }

                                if (filterValues == undefined || (filter.FilterSettings != undefined && (filterValues[filter.FilterSettings.FieldName] == undefined || !filterValues[filter.FilterSettings.FieldName].isHidden))) {
                                    var filterItem = {
                                        payload: filter,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromisedeferred: UtilsService.createPromiseDeferred(),
                                    };

                                    promises.push(filterItem.loadPromisedeferred.promise);
                                    addFilterAPI(filterItem);
                                }
                            }
                        }

                    }

                    function addFilterAPI(filterItem) {
                        var filter = {
                            editor: filterItem.payload.FilterSettings.RuntimeEditor,
                            showInBasic: filterItem.payload.ShowInBasic
                        };
                        filter.onFilterReady = function (api) {
                            filter.filterAPI = api;
                            filterItem.readyPromiseDeferred.resolve();
                        };

                        filterItem.readyPromiseDeferred.promise.then(function () {

                            var filterDirectivePayload = {
                                settings: filterItem.payload.FilterSettings,
                                dataRecordTypeId: dataRecordTypeId,
                                genericContext: genericContext,
                                allFieldValuesByName: allFieldValuesByName,
                                isFromManagementScreen: isFromManagementScreen
                            };
                            VRUIUtilsService.callDirectiveLoad(filter.filterAPI, filterDirectivePayload, filterItem.loadPromisedeferred);
                        });
                        $scope.scopeModel.filters.push(filter);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function (filterObject) {
                    var filterData = {};
                    var recordFilters = [];
                    var filters = [];

                    for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                        var filter = $scope.scopeModel.filters[i];
                        var data = filter.filterAPI != undefined ? filter.filterAPI.getData() : undefined;
                        if (data != undefined) {
                            if (data.RecordFilter != undefined)
                                recordFilters.push(data.RecordFilter);
                            if (data.FromTime != undefined)
                                filterData.FromTime = data.FromTime;
                            if (data.ToTime != undefined)
                                filterData.ToTime = data.ToTime;
                            if (data.LimitResult != undefined)
                                filterData.LimitResult = data.LimitResult;

                            if (data.Filters != undefined) {
                                for (var j = 0; j < data.Filters.length; j++) {
                                    filters.push(data.Filters[j]);
                                }
                            }
                        }
                    }
                    filterData.RecordFilter = {
                        $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                        LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                        Filters: recordFilters
                    };
                    filterData.Filters = filters;

                    return filterData;
                };

                api.hasFilters = function () {
                    var filterCount = $scope.scopeModel.filters.length;
                    if (filterCount == 0)
                        return false;

                    for (var i = 0; i < filterCount; i++) {
                        var filter = $scope.scopeModel.filters[i];
                        var hasFilters = filter.filterAPI.hasFilters();
                        if (hasFilters)
                            return true;
                    }

                    return false;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];

                    for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                        var filter = $scope.scopeModel.filters[i];
                        if (filter.filterAPI != undefined && filter.filterAPI.onFieldValueChanged != undefined && typeof (filter.filterAPI.onFieldValueChanged) == "function") {
                            var onFieldValueChangedPromise = filter.filterAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (onFieldValueChangedPromise != undefined)
                                _promises.push(onFieldValueChangedPromise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeBasicadvanced', BasicAdvancedFilterRuntimeSettingsDirective);

})(app);