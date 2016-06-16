(function (app) {

    'use strict';

    RecordSearchAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_OrderDirectionEnum', 'VRValidationService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService','PeriodEnum'];

    function RecordSearchAnalyticReportDirective(UtilsService, VRUIUtilsService, VR_Analytic_OrderDirectionEnum, VRValidationService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService, PeriodEnum) {
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

            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTimeRangeDirectiveReady = function (api) {
                    timeRangeDirectiveAPI = api;
                    timeRangeReadyPromiseDeferred.resolve();
                }

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.search = function () {
                    setGridQuery();
                    return gridAPI.loadGrid(gridQuery);
                };

                $scope.addFilter = function () {
                    if ($scope.selectedDRSearchPageStorageSource != undefined) {
                        $scope.scopeModel.isLoading = true;
                        loadFields().then(function (response) {
                            if (response) {
                                var fields = [];
                                for (var i = 0; i < response.length; i++) {
                                    var dataRecordField = response[i];
                                    fields.push({
                                        FieldName: dataRecordField.Entity.Name,
                                        FieldTitle: dataRecordField.Entity.Title,
                                        Type: dataRecordField.Entity.Type,
                                    });
                                }
                                $scope.scopeModel.isLoading = false;
                                var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                                    filterObj = filter;
                                    $scope.expression = expression;
                                }
                                VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, filterObj, onDataRecordFieldTypeFilterAdded);
                            }
                        });
                    }
                };
                $scope.checkMaxNumberResords = function()
                {
                    if($scope.limit <=  $scope.maxNumberOfRecords || $scope.maxNumberOfRecords == undefined)
                    {
                        return null;
                    }
                    else
                    {
                        return "Max number can be entered is: " + $scope.maxNumberOfRecords;
                    }
                }
                $scope.onDRSearchPageStorageSourceChanged = function () {
                    $scope.expression = undefined;
                    filterObj = null;
                }

                $scope.resetFilter = function () {
                    $scope.expression = undefined;
                    filterObj = null;
                }

                $scope.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload.settings;
                    }
                    var loadPromiseDeffer = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultipleAsyncOperations([setSourceSelector, setStaticData, loadTimeRangeDirective]).then(function () {
                        loadPromiseDeffer.resolve();
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
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter);
            }

            function loadTimeRangeDirective() {
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                timeRangeReadyPromiseDeferred.promise.then(function () {
                    var timeRangePeriod = {
                        period: PeriodEnum.Today.value
                    };

                    VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeDimentionPromiseDeferred);

                });
                return loadTimeDimentionPromiseDeferred.promise;
            }

            function setSourceSelector() {
                if (settings != undefined)
                    $scope.drSearchPageStorageSources = settings.Sources;
            }
            function setStaticData() {
                $scope.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);
                $scope.selectedOrderDirection = $scope.orderDirectionList[0];
                $scope.fromDate = new Date();
                $scope.limit = settings != undefined ? settings.NumberOfRecords : 100;
                $scope.maxNumberOfRecords = settings != undefined ? settings.MaxNumberOfRecords : undefined;
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