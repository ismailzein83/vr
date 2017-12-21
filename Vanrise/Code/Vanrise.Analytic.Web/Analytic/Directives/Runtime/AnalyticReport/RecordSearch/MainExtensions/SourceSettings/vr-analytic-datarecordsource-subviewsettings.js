(function (app) {

    'use strict';

    AnalyticDataRecordSourceSubviewSettingsDirective.$inject = ['UtilsService', 'VR_Analytic_AnalyticReportAPIService'];

    function AnalyticDataRecordSourceSubviewSettingsDirective(UtilsService, VR_Analytic_AnalyticReportAPIService) {
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

            var subviewDefinition;
            var parentSearchQuery;
            var dataRecordStorageLog;
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
                        subviewDefinition = payload.subviewDefinition;
                        parentSearchQuery = payload.parentSearchQuery;
                        dataRecordStorageLog = payload.dataRecordStorageLog;
                    }

                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                    getReportDefinition(subviewDefinition.Settings.AnalyticReportId).then(function () {
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
                    LimitResult: parentSearchQuery.LimitResult,
                    FromTime: parentSearchQuery.FromTime,
                    ToTime: parentSearchQuery.ToTime,
                    //Direction: $scope.selectedOrderDirection.value,
                    //sortDirection: $scope.selectedOrderDirection.sortDirection,
                };
            }
            function buildMappingFilters() {
                var filters = [];

                for (var i = 0; i < subviewDefinition.Settings.Mappings.length; i++) {
                    var currentMapping = subviewDefinition.Settings.Mappings[i];

                    for (var fieldValueName in dataRecordStorageLog.FieldValues) {
                        if (fieldValueName == currentMapping.ParentColumnName) {
                            var currentFieldValue = dataRecordStorageLog.FieldValues[fieldValueName];
                            filters.push({ FieldName: currentMapping.SubviewColumnName, FilterValues: [currentFieldValue.Value] });
                        }
                    }
                }

                return filters;
            }
        }
    }

    app.directive('vrAnalyticDatarecordsourceSubviewsettings', AnalyticDataRecordSourceSubviewSettingsDirective);

})(app);