(function (app) {

    'use strict';

    RecordSearchAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_OrderDirectionEnum', 'VRValidationService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService', 'PeriodEnum', 'VR_Analytic_AnalyticAPIService', 'VR_GenericData_RecordFilterAPIService', 'VR_GenericData_DataRecordStorageAPIService', 'UISettingsService', 'VR_Analytic_AnalyticReportAPIService'];

    function RecordSearchAnalyticReportDirective(UtilsService, VRUIUtilsService, VR_Analytic_OrderDirectionEnum, VRValidationService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService, PeriodEnum, VR_Analytic_AnalyticAPIService, VR_GenericData_RecordFilterAPIService, VR_GenericData_DataRecordStorageAPIService, UISettingsService, VR_Analytic_AnalyticReportAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recordSearchAnalyticReport = new RecordSearchAnalyticReport($scope, ctrl, $attrs);
                recordSearchAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RecordSearch/Templates/RecordSearchAnalyticReportRuntimeTemplates.html"
        };
        function RecordSearchAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var filterObj;
            var settings;
            var autoSearch;
            var itemActionSettings;
            var preDefinedFilter;
            var analyticReportId;

            var fromDate;
            var toDate;
            var period;
            var sourceName;
            var reportName;

            var fields = [];
            var fieldTypes = [];

            var gridAPI;

            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectedDRSearchPageStorageSource = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.showSourceSelector = false;
                $scope.filters = [];

                $scope.onTimeRangeDirectiveReady = function (api) {
                    timeRangeDirectiveAPI = api;
                    timeRangeReadyPromiseDeferred.resolve();
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.onDRSearchPageStorageSourceChanged = function () {
                    filterObj = null;
                    $scope.expression = undefined;
                    if ($scope.selectedDRSearchPageStorageSource != undefined) {
                        if (selectedDRSearchPageStorageSource != undefined)
                            selectedDRSearchPageStorageSource.resolve();
                        else {
                            $scope.isloadingFilter = true;
                            loadFields().then(function () {
                                loadFilters().then(function () {
                                    $scope.isloadingFilter = false;
                                });
                            });
                        }

                    }

                };

                $scope.search = function () {
                    return gridAPI.loadGrid(getGridQuery());
                };

                $scope.addFilter = function () {
                    if ($scope.selectedDRSearchPageStorageSource != undefined) {
                        if (fields.length > 0) {
                            var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                                filterObj = filter;
                                $scope.expression = expression;
                            };
                            VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, filterObj, onDataRecordFieldTypeFilterAdded);
                        }
                    }
                };

                $scope.checkMaxNumberResords = function () {
                    if ($scope.limit <= $scope.maxNumberOfRecords || $scope.maxNumberOfRecords == undefined) {
                        return null;
                    }
                    else {
                        return "Max number can be entered is: " + $scope.maxNumberOfRecords;
                    }
                };

                $scope.resetFilter = function () {
                    $scope.expression = undefined;
                    filterObj = null;
                };

                $scope.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload.settings;
                        reportName = payload.ReportName ? payload.ReportName : "Analytic Data Record Storage";
                        autoSearch = payload.autoSearch;
                        itemActionSettings = payload.itemActionSettings;
                        analyticReportId = payload.analyticReportId;
                        if (itemActionSettings != undefined) {
                            fromDate = itemActionSettings.FromDate;
                            toDate = itemActionSettings.ToDate;
                            period = itemActionSettings.Period;
                            sourceName = itemActionSettings.SourceName;
                        }
                        else {
                            preDefinedFilter = payload.preDefinedFilter;
                            if (preDefinedFilter != undefined) {
                                fromDate = preDefinedFilter.FromDate;
                                toDate = preDefinedFilter.ToDate;
                                period = preDefinedFilter.Period;
                                sourceName = preDefinedFilter.SourceName;
                            }
                        }
                    }

                    var loadPromiseDeffer = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultipleAsyncOperations([setSourceSelector, setStaticData, loadTimeRangeDirective, getFieldTypeConfigs]).then(function () {
                        loadFields().then(function () {
                            loadFilters().then(function () {
                                if (itemActionSettings != undefined) {
                                    var input = {
                                        DimensionFilters: itemActionSettings.DimensionFilters,
                                        ReportId: itemActionSettings.AnalyticReportId,
                                        SourceName: itemActionSettings.SourceName,
                                        TableId: itemActionSettings.TableId,
                                        FilterGroup: itemActionSettings.FilterGroup
                                    };
                                    VR_Analytic_AnalyticAPIService.GetRecordSearchFilterGroup(input).then(function (response) {
                                        filterObj = response;
                                        buildRecordFilterGroupExpression();
                                    }).catch(function (error) {
                                        loadPromiseDeffer.reject(error);
                                    });

                                } else if (preDefinedFilter != undefined) {

                                    var input = {
                                        FieldFilters: preDefinedFilter.FieldFilters,
                                        ReportId: preDefinedFilter.AnalyticReportId,
                                        SourceName: preDefinedFilter.SourceName,
                                    };
                                    VR_Analytic_AnalyticAPIService.GetRecordSearchFieldFilter(input).then(function (response) {
                                        filterObj = response;
                                        buildRecordFilterGroupExpression();
                                    }).catch(function (error) {
                                        loadPromiseDeffer.reject(error);
                                    });

                                } else {
                                    loadPromiseDeffer.resolve();
                                }
                            });

                        });
                        function buildRecordFilterGroupExpression() {
                            if (filterObj != undefined) {
                                var buildRecordFilterGroupExpressionInput = {
                                    RecordFilterFieldInfosByFieldName: buildRecordFilterFieldInfosByFieldName(fields),
                                    FilterGroup: filterObj
                                };
                                VR_GenericData_RecordFilterAPIService.BuildRecordFilterGroupExpression(buildRecordFilterGroupExpressionInput).then(function (response) {
                                    $scope.expression = response;
                                    loadPromiseDeffer.resolve();
                                }).catch(function (error) {
                                    loadPromiseDeffer.reject(error);
                                });
                            } else {
                                loadPromiseDeffer.resolve();
                            }
                        }

                    }).catch(function (error) {
                        loadPromiseDeffer.reject(error);
                    });
                    loadPromiseDeffer.promise.then(function () {
                        if (autoSearch) {
                            gridAPI.loadGrid(getGridQuery());
                        }
                        selectedDRSearchPageStorageSource = undefined;
                    });
                    return loadPromiseDeffer.promise;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getFieldTypeConfigs() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    fieldTypes.length = 0;
                    for (var i = 0; i < response.length; i++) {
                        fieldTypes.push(response[i]);
                    }
                });
            }

            function loadFilters() {
                var filterPromises = [];
                $scope.filters.length = 0;
                if ($scope.selectedDRSearchPageStorageSource != undefined && settings != undefined && settings.Sources != undefined) {
                    var source = UtilsService.getItemByVal(settings.Sources, $scope.selectedDRSearchPageStorageSource.Name, "Name");
                    if (source != undefined && source.Filters != undefined) {
                        for (var i = 0; i < source.Filters.length; i++) {
                            var filterConfiguration = source.Filters[i];
                            var filter = getFilter(filterConfiguration);
                            if (filter != undefined) {
                                filterPromises.push(filter.directiveLoadDeferred.promise);
                                $scope.filters.push(filter);
                            }
                        }
                    }
                    function getFilter(filterConfiguration) {
                        var field = UtilsService.getItemByVal(fields, filterConfiguration.FieldName, 'FieldName');
                        var filter;
                        var filterEditor;
                        var fieldType;
                        if (field != undefined) {
                            fieldType = UtilsService.getItemByVal(fieldTypes, field.Type.ConfigId, 'ExtensionConfigurationId');
                        }
                        if (fieldType != undefined) {
                            filterEditor = fieldType.FilterEditor;
                        }
                        if (filterEditor == null) return filter;

                        filter = {};
                        filter.fieldName = filterConfiguration.FieldName;
                        filter.isRequired = filterConfiguration.IsRequired;
                        filter.directiveEditor = filterEditor;
                        filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        filter.onDirectiveReady = function (api) {
                            filter.directiveAPI = api;
                            var directivePayload = {
                                fieldTitle: filterConfiguration.FieldTitle,
                                fieldType: field != undefined ? field.Type : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                        };

                        return filter;
                    }
                }
                return UtilsService.waitMultiplePromises(filterPromises);
            }

            function setSourceSelector() {
                $scope.drSearchPageStorageSources = [];
                if (settings != undefined) {
                    return VR_Analytic_AnalyticReportAPIService.CheckRecordStoragesAccess(analyticReportId).then(function (response) {
                        if (response != undefined && response.length > 0) {
                            for (var i = 0; i < response.length; i++) {

                                var currenctSourceName = response[i];
                                var source = UtilsService.getItemByVal(settings.Sources, currenctSourceName, "Name");
                                if (source != undefined)
                                    $scope.drSearchPageStorageSources.push(source);
                            }

                            if (sourceName != undefined) {
                                $scope.selectedDRSearchPageStorageSource = UtilsService.getItemByVal($scope.drSearchPageStorageSources, sourceName, "Name");
                            } else if ($scope.drSearchPageStorageSources.length > 0) {
                                $scope.selectedDRSearchPageStorageSource = $scope.drSearchPageStorageSources[0];
                            }

                            if ($scope.drSearchPageStorageSources.length > 1) {
                                $scope.showSourceSelector = true;
                            }
                        }
                    });
                }

            }



            function setStaticData() {
                $scope.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);
                $scope.selectedOrderDirection = $scope.orderDirectionList[1];
                $scope.limit = settings != undefined ? settings.NumberOfRecords : 100;
                $scope.maxNumberOfRecords = settings != undefined && settings.MaxNumberOfRecords != null ? settings.MaxNumberOfRecords : UISettingsService.getMaxSearchRecordCount();
            }



            function loadTimeRangeDirective() {
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                timeRangeReadyPromiseDeferred.promise.then(function () {
                    var timeRangePeriod = {
                        period: period != undefined ? period : PeriodEnum.Today.value,
                        fromDate: fromDate,
                        toDate: toDate
                    };

                    VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeDimentionPromiseDeferred);

                });
                return loadTimeDimentionPromiseDeferred.promise;
            }

            function loadFields() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo($scope.selectedDRSearchPageStorageSource.DataRecordTypeId).then(function (response) {
                    if (response) {
                        fields.length = 0;
                        for (var i = 0; i < response.length; i++) {
                            var dataRecordField = response[i];
                            fields.push({
                                FieldName: dataRecordField.Entity.Name,
                                FieldTitle: dataRecordField.Entity.Title,
                                Type: dataRecordField.Entity.Type,
                            });
                        }
                    }
                });
            }
            function checkIfAllow(tab1, tab2) {
                for (var i = 0; i < tab1.length; i++) {
                    if (tab2.indexOf(tab1[i]) === -1)
                        return false;
                }
                return true;
            }
            function getGridQuery() {
                var filters = [];
                if ($scope.filters != undefined) {
                    for (var i = 0; i < $scope.filters.length; i++) {
                        var filter = $scope.filters[i];
                        if (filter.directiveAPI != undefined && filter.directiveAPI.getData() != undefined) {
                            filters.push({
                                FieldName: filter.fieldName,
                                FilterValues: filter.directiveAPI.getValuesAsArray()
                            });
                        }
                    }
                }

                return {
                    DataRecordStorageIds: $scope.selectedDRSearchPageStorageSource.RecordStorageIds,
                    ReportName: reportName,
                    DataRecordTypeId: $scope.selectedDRSearchPageStorageSource.DataRecordTypeId,
                    GridColumns: $scope.selectedDRSearchPageStorageSource.GridColumns,
                    ItemDetails: $scope.selectedDRSearchPageStorageSource.ItemDetails,
                    SubviewDefinitions: $scope.selectedDRSearchPageStorageSource.SubviewDefinitions,
                    SortColumns: $scope.selectedDRSearchPageStorageSource.SortColumns,
                    FilterGroup: buildFilterGroupObj(filterObj, $scope.selectedDRSearchPageStorageSource.RecordFilter),
                    Filters: filters,
                    LimitResult: $scope.limit,
                    FromTime: $scope.fromDate,
                    ToTime: $scope.toDate,
                    Direction: $scope.selectedOrderDirection.value,
                    sortDirection: $scope.selectedOrderDirection.sortDirection
                };
            }

            function buildFilterGroupObj(filterObj, sourceRecordFilter) {
                if (sourceRecordFilter == undefined)
                    return filterObj;

                if (filterObj == undefined)
                    return sourceRecordFilter;

                return {
                    $type: 'Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities',
                    LogicalOperator: 0,
                    Filters: [filterObj, sourceRecordFilter]
                };
            }
            function buildRecordFilterFieldInfosByFieldName(recordFields) {
                if (recordFields == undefined)
                    return;

                var recordFilterFieldInfosByFieldName = {};

                for (var index = 0; index < recordFields.length; index++) {
                    var recordField = recordFields[index];
                    recordFilterFieldInfosByFieldName[recordField.FieldName] = { Name: recordField.FieldName, Title: recordField.FieldTitle, Type: recordField.Type };
                }

                return recordFilterFieldInfosByFieldName;
            }
        }
    }

    app.directive('vrAnalyticAnalyticreportRecordsearchRuntime', RecordSearchAnalyticReportDirective);

})(app);