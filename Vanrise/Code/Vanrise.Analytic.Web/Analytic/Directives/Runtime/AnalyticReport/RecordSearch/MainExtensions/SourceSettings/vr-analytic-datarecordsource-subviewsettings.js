(function (app) {

    'use strict';

    AnalyticDataRecordSourceSubviewSettingsDirective.$inject = ['UtilsService', 'VR_Analytic_AnalyticReportAPIService', 'VR_GenericData_DataRecordTypeAPIService'];

    function AnalyticDataRecordSourceSubviewSettingsDirective(UtilsService, VR_Analytic_AnalyticReportAPIService, VR_GenericData_DataRecordTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AnalyticDataRecordSourceSubviewSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RecordSearch/MainExtensions/SourceSettings/Templates/DataRecordSourceSubviewSettingsTemplate.html"
        };

        function AnalyticDataRecordSourceSubviewSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordStorageLog;
            var subviewDefinition;
            var parentDataRecordTypeId;
            var limitResult;
            var dateTimeFieldValue;
            var analyticReportSource;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        dataRecordStorageLog = payload.dataRecordStorageLog;
                        subviewDefinition = payload.subviewDefinition;
                        parentDataRecordTypeId = payload.parentDataRecordTypeId;
                        limitResult = payload.limitResult;
                    }

                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var promises = [];
                    promises.push(getReportDefinition(subviewDefinition.Settings.AnalyticReportId));

                    if (subviewDefinition.Settings.IncludeTimeFilter) {
                        promises.push(getDateTimeFieldValue());
                    }

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        var query = getGridQuery();
                        gridAPI.loadGrid(query);

                        loadPromiseDeferred.resolve();
                    }).catch(function (error) {
                        loadPromiseDeferred.reject(error);
                    });

                    return loadPromiseDeferred.promise;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function getReportDefinition(reportId) {
                return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportById(reportId).then(function (reportEntity) {
                    if (reportEntity && reportEntity.Settings && reportEntity.Settings.Sources) {
                        analyticReportSource = UtilsService.getItemByVal(reportEntity.Settings.Sources, subviewDefinition.Settings.DRSourceName, "Name");
                    }
                });
            }
            function getDateTimeFieldValue() {
                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(parentDataRecordTypeId).then(function (dataRecordType) {
                    var dateTimeFieldName = dataRecordType.Settings.DateTimeField;
                    if (dateTimeFieldName == undefined)
                        return;

                    if (dataRecordStorageLog.FieldValues[dateTimeFieldName] == undefined)
                        return;

                    dateTimeFieldValue = dataRecordStorageLog.FieldValues[dateTimeFieldName].Value;
                });
            }

            function getGridQuery() {

                return {
                    DataRecordStorageIds: analyticReportSource.RecordStorageIds,
                    DataRecordTypeId: analyticReportSource.DataRecordTypeId,
                    GridColumns: analyticReportSource.GridColumns,
                    ItemDetails: analyticReportSource.ItemDetails,
                    SubviewDefinitions: analyticReportSource.SubviewDefinitions,
                    SortColumns: analyticReportSource.SortColumns,
                    FilterGroup: analyticReportSource.RecordFilter,
                    Filters: buildMappingFilters(),
                    LimitResult: limitResult,
                    FromTime: dateTimeFieldValue,
                    ToTime: dateTimeFieldValue,
                    //Direction: $scope.selectedOrderDirection.value,
                    //sortDirection: $scope.selectedOrderDirection.sortDirection,
                };
            }
            function buildMappingFilters() {
                var filters = [];

                for (var i = 0; i < subviewDefinition.Settings.Mappings.length; i++) {
                    var currentMapping = subviewDefinition.Settings.Mappings[i];
                    var currentFilter = { FieldName: currentMapping.SubviewColumnName };

                    var currentFieldValue = dataRecordStorageLog.FieldValues[currentMapping.ParentColumnName];
                    if (currentFieldValue != undefined && currentFieldValue.Value != undefined){
                        currentFilter.FilterValues = [currentFieldValue.Value];
                    }
                    else {
                        currentFilter.FilterValues = null;
                    }

                    filters.push(currentFilter);
                }

                return filters;
            }
        }
    }

    app.directive('vrAnalyticDatarecordsourceSubviewsettings', AnalyticDataRecordSourceSubviewSettingsDirective);

})(app);