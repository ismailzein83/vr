(function (app) {

    'use strict';

    RecordSearchAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_OrderDirectionEnum', 'VRValidationService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService', 'PeriodEnum', 'VR_Analytic_AnalyticAPIService', 'VR_GenericData_RecordFilterAPIService', 'VR_GenericData_DataRecordStorageAPIService'];

    function RecordSearchAnalyticReportDirective(UtilsService, VRUIUtilsService, VR_Analytic_OrderDirectionEnum, VRValidationService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService, PeriodEnum, VR_Analytic_AnalyticAPIService, VR_GenericData_RecordFilterAPIService, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
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
            var gridQuery;
            var gridAPI;
            var autoSearch;
            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var fields = [];

            var itemActionSettings;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTimeRangeDirectiveReady = function (api) {
                    timeRangeDirectiveAPI = api;
                    timeRangeReadyPromiseDeferred.resolve();
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (autoSearch) {
                        setGridQuery();
                        gridAPI.loadGrid(gridQuery);
                    }
                };

                $scope.search = function () {
                    setGridQuery();
                    return gridAPI.loadGrid(gridQuery);
                };

                $scope.addFilter = function () {
                    if ($scope.selectedDRSearchPageStorageSource != undefined) {
                        if (fields.length > 0)
                        {
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
                $scope.onDRSearchPageStorageSourceChanged = function () {
                    filterObj = null;
                    $scope.expression = undefined;
                    if ($scope.selectedDRSearchPageStorageSource != undefined) {
                        $scope.isloadingFilter = true;
                        loadFields().then(function () {

                            $scope.isloadingFilter = false;

                        });
                    }

                };

                $scope.resetFilter = function () {
                    $scope.expression = undefined;
                    filterObj = null;
                };

                $scope.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload.settings;
                        autoSearch = payload.autoSearch;
                        itemActionSettings = payload.itemActionSettings;
                    }
                    var loadPromiseDeffer = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultipleAsyncOperations([setSourceSelector, setStaticData, loadTimeRangeDirective]).then(function () {
                       

                        if (itemActionSettings != undefined) {
                            
                            loadFields().then(function () {
                                var input = {
                                    DimensionFilters: itemActionSettings.DimensionFilters,
                                    ReportId: itemActionSettings.AnalyticReportId,
                                    SourceName: itemActionSettings.SourceName,
                                    TableId: itemActionSettings.TableId,
                                    FilterGroup: itemActionSettings.FilterGroup
                                };
                                VR_Analytic_AnalyticAPIService.GetRecordSearchFilterGroup(input).then(function (response) {
                                    filterObj = response;
                                    var recordFields = [];
                                    for (var i = 0; i < fields.length; i++) {
                                        var field = fields[i];
                                        recordFields.push({
                                            Name: field.FieldName,
                                            Type: field.Type,
                                        });
                                    };

                                    VR_GenericData_RecordFilterAPIService.BuildRecordFilterGroupExpression({ RecordFields: recordFields, FilterGroup: filterObj }).then(function (response) {
                                        $scope.expression = response;
                                        loadPromiseDeffer.resolve();
                                    }).catch(function (error) {
                                        loadPromiseDeffer.reject(error);
                                    });
                                }).catch(function (error) {
                                    loadPromiseDeffer.reject(error);
                                });
                            })
                        } else
                        {
                            loadPromiseDeffer.resolve();
                        }

                        if (autoSearch && gridAPI !=undefined)
                        {
                            gridAPI.loadGrid(gridQuery);
                        }
                    }).catch(function (error) {
                        loadPromiseDeffer.reject(error);
                    });
                    return loadPromiseDeffer.promise;
                };


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }

            function loadFields() {
                var obj = { DataRecordTypeId: $scope.selectedDRSearchPageStorageSource.DataRecordTypeId };
                var serializedFilter = UtilsService.serializetoJson(obj);
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter).then(function (response) {
                    if (response) {
                        fields.length = 0;
                        for (var i = 0; i < response.length; i++) {
                            var dataRecordField = response[i];
                            fields.push({
                                FieldName: dataRecordField.Entity.Name,
                                FieldTitle: dataRecordField.Entity.Title,
                                Type: dataRecordField.Entity.Type,
                            });
                        };
                    }
                });
            }

            function loadTimeRangeDirective() {
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                timeRangeReadyPromiseDeferred.promise.then(function () {
                    var timeRangePeriod = {
                        period: PeriodEnum.Today.value,
                        fromDate: itemActionSettings != undefined ? itemActionSettings.FromDate : undefined,
                        toDate:itemActionSettings !=undefined?itemActionSettings.ToDate:undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeDimentionPromiseDeferred);

                });
                return loadTimeDimentionPromiseDeferred.promise;
            }

            function setSourceSelector() {
                var tabids = [];
                $scope.drSearchPageStorageSources =[];
                if (settings != undefined) {
                    // $scope.drSearchPageStorageSources = settings.Sources;
                    for (var i = 0; i < settings.Sources.length ; i++) {
                        for (var j = 0; j < settings.Sources[i].RecordStorageIds.length ; j++) {
                            var id = settings.Sources[i].RecordStorageIds[j];
                            if (tabids.indexOf(id) == -1)
                                tabids[tabids.length] = id;
                        }
                    }

                   return VR_GenericData_DataRecordStorageAPIService.CheckRecordStoragesAccess(tabids).then(function (response) {
                        for (var i = 0; i < settings.Sources.length ; i++) {
                            var neededIds = settings.Sources[i].RecordStorageIds ;
                            if(checkIfAllow(settings.Sources[i].RecordStorageIds,response))
                                $scope.drSearchPageStorageSources[$scope.drSearchPageStorageSources.length] = settings.Sources[i]
                        }
                    });
                }
                if (itemActionSettings != undefined) {
                    $scope.selectedDRSearchPageStorageSource = UtilsService.getItemByVal($scope.drSearchPageStorageSources, itemActionSettings.SourceName, "Name");
                }


               
            }
            function setStaticData() {
                $scope.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);
                $scope.selectedOrderDirection = $scope.orderDirectionList[0];
             //   $scope.fromDate = new Date();
                $scope.limit = settings != undefined ? settings.NumberOfRecords : 100;
                $scope.maxNumberOfRecords = settings != undefined ? settings.MaxNumberOfRecords : undefined;

               
            }
            function checkIfAllow(tab1, tab2) {
                for (var i = 0; i < tab1.length; i++) {
                    if (tab2.indexOf(tab1[i]) === -1)
                        return false;
                }
                return true;
            }
            function setGridQuery() {
                gridQuery = {
                    DataRecordStorageIds: $scope.selectedDRSearchPageStorageSource.RecordStorageIds,
                    FromTime: $scope.fromDate,
                    ToTime: $scope.toDate,
                    GridColumns: $scope.selectedDRSearchPageStorageSource.GridColumns,
                    ItemDetails: $scope.selectedDRSearchPageStorageSource.ItemDetails,
                    SortColumns: $scope.selectedDRSearchPageStorageSource.SortColumns,
                    FilterGroup: filterObj,
                    LimitResult: $scope.limit,
                    Direction: $scope.selectedOrderDirection.value,
                    sortDirection: $scope.selectedOrderDirection.sortDirection,
                    DataRecordTypeId: $scope.selectedDRSearchPageStorageSource.DataRecordTypeId
                };
            }
        }
    }
    app.directive('vrAnalyticAnalyticreportRecordsearchRuntime', RecordSearchAnalyticReportDirective);
})(app);