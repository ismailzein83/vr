(function (app) {

    'use strict';

    HistoryAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Analytic_AnalyticService'];

    function HistoryAnalyticReportDirective(UtilsService, VRUIUtilsService, Analytic_AnalyticService) {
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
                var historyAnalyticReport = new HistoryAnalyticReport($scope, ctrl, $attrs);
                historyAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/History/Templates/HistoryAnalyticReportTemplates.html"

        };
        function HistoryAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var searchSettingDirectiveAPI;
            var searchSettingReadyDeferred;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.widgetsGridMenuActions = function (dataItem) {
                    return getwidgetGridMenuActions();
                };

                $scope.scopeModel.widgets = [];
              
                $scope.scopeModel.addWidget = function () {
                    var onWidgetAdd = function (widget) {
                        $scope.scopeModel.widgets.push({ widgetSettings: widget });
                    };
                    Analytic_AnalyticService.addWidget(onWidgetAdd, tableSelectorAPI.getSelectedIds());
                };

                $scope.scopeModel.removeWidget = function (dataItem) {
                    var datasourceIndex = $scope.scopeModel.widgets.indexOf(dataItem);
                    $scope.scopeModel.widgets.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.isWidgetValid = function () {
                    if ($scope.scopeModel.widgets.length > 0)
                        return null;
                    return "At least one widget should be selected.";
                };

                $scope.scopeModel.onTableSelectionChanged = function () {
                    if (searchSettingDirectiveAPI != undefined && tableSelectorAPI != undefined) {
                        var setLoader = function (value) { $scope.scopeModel.isLoadingSearchSettingsDirective = value };
                        var payload = {
                            tableIds: tableSelectorAPI.getSelectedIds()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, searchSettingDirectiveAPI, payload, setLoader, searchSettingReadyDeferred);
                    }
                };

                $scope.scopeModel.selectedTables = [];

                $scope.scopeModel.onSearchSettingsDirectiveReady = function (api) {
                    searchSettingDirectiveAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingSearchSettingsDirective = value };
                    var payload = {
                        tableIds: tableSelectorAPI.getSelectedIds()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, searchSettingDirectiveAPI, payload, setLoader, searchSettingReadyDeferred);
                };

                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var reportSettings;
                    var promises = [];
                    if(payload != undefined && payload.reportSettings != undefined) {
                        reportSettings = payload.reportSettings;
                      
                        if (reportSettings != undefined  && reportSettings.Widgets != undefined && reportSettings.Widgets.length > 0) {
                            for (var i = 0 ; i < reportSettings.Widgets.length; i++) {
                                $scope.scopeModel.widgets.push({ widgetSettings: reportSettings.Widgets[i] });
                            }
                        }
                        searchSettingReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadSearchSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                        searchSettingReadyDeferred.promise.then(function () {
                            searchSettingReadyDeferred = undefined;
                            var payLoad;
                            if (reportSettings != undefined) {
                                payLoad = {
                                    tableIds: reportSettings.AnalyticTableIds,
                                    searchSettings: reportSettings.SearchSettings
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(searchSettingDirectiveAPI, payLoad, loadSearchSettingsPromiseDeferred);
                        });
                        promises.push(loadSearchSettingsPromiseDeferred.promise);
                    }
                    var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    tableSelectorReadyDeferred.promise.then(function () {
                        var payLoad;
                        if (reportSettings != undefined) {
                            payLoad = {
                                selectedIds: reportSettings.AnalyticTableIds
                            }
                        }

                        VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoad, loadTableSelectorPromiseDeferred);
                    });
                    promises.push(loadTableSelectorPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var widgets = [];
                    if ($scope.scopeModel.widgets != undefined && $scope.scopeModel.widgets.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.widgets.length; i++) {
                            widgets.push($scope.scopeModel.widgets[i].widgetSettings);
                        }
                    }

                    var settings = {
                        $type: "Vanrise.Analytic.Entities.AnalyticHistoryReportSettings, Vanrise.Analytic.Entities",
                        AnalyticTableIds: tableSelectorAPI != undefined ? tableSelectorAPI.getSelectedIds() : undefined,
                        SearchSettings: searchSettingDirectiveAPI != undefined ? searchSettingDirectiveAPI.getData() : undefined,
                        Widgets: widgets
                    };

                    return settings;
                }
            }

            function editWidget(dataItem) {
                var onWidgetUpdated = function (widgetObj) {
                    var index = $scope.scopeModel.widgets.indexOf(dataItem);
                    $scope.scopeModel.widgets[index].widgetSettings = widgetObj;
                };
                Analytic_AnalyticService.editWidget(dataItem.widgetSettings, onWidgetUpdated, tableSelectorAPI.getSelectedIds());

            }
       
            function getwidgetGridMenuActions() {
                var defaultMenuActions = [
                  {
                      name: "Edit",
                      clicked: editWidget,
                  }];
                return defaultMenuActions;
            }
        }
    }
    app.directive('vrAnalyticAnalyticreportHistoryDefinition', HistoryAnalyticReportDirective);
})(app);