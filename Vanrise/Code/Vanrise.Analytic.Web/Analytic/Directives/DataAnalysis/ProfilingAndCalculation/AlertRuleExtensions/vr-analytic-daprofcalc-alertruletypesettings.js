(function (app) {

    'use strict';

    DAProfCalcAlertRuleTypeSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcAlertRuleTypeSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrDAProfCalcAlertRuleTypeSettings = new VRDAProfCalcAlertRuleTypeSettings($scope, ctrl, $attrs);
                vrDAProfCalcAlertRuleTypeSettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleTypeSettingsTemplate.html"

        };
        function VRDAProfCalcAlertRuleTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataAnalysisDefinitionSelectorAPI;
            var dataAnalysisDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedDataAnalysisDefinitionSelectoReadyDeferred;

            var sourceDataRecordStorageSelectorAPI;
            var sourceDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisDefinitionSelectorReady = function (api) {
                    dataAnalysisDefinitionSelectorAPI = api;
                    dataAnalysisDefinitionSelectoReadyDeferred.resolve();
                };

                $scope.onSourceDataRecordStorageSelectorReady = function (api) {
                    sourceDataRecordStorageSelectorAPI = api;
                    sourceDataRecordStorageSelectorReadyDeferred.resolve();
                };

                $scope.onDataAnalysisDefinitionSelectionChanged = function (dataItem) {
                    if (dataItem != undefined) {
                        var filters = [];
                        var daProfCalcDataRecordStorageFilter = {
                            $type: 'Vanrise.Analytic.MainExtensions.DataAnalysis.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.MainExtensions',
                            DataAnalysisDefinitionId: dataItem.DataAnalysisDefinitionId
                        };
                        filters.push(daProfCalcDataRecordStorageFilter);

                        var setSourceLoader = function (value) { $scope.scopeModel.isLoadingSourceDataRecordStorage = value };
                        var payload = {
                            filters: filters
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceDataRecordStorageSelectorAPI, payload, setSourceLoader, selectedDataAnalysisDefinitionSelectoReadyDeferred);
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var vrAlertRuleTypeSettings;

                    if (payload != undefined)
                        vrAlertRuleTypeSettings = payload.settings;

                    if (vrAlertRuleTypeSettings != undefined) {
                        selectedDataAnalysisDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();
                    }
                    var dataAnalysisDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dataAnalysisDefinitionSelectoReadyDeferred.promise.then(function () {
                        var dataAnalysisDefinitionSelectorPayload;

                        if (vrAlertRuleTypeSettings != undefined) {
                            dataAnalysisDefinitionSelectorPayload = {
                                selectedIds: vrAlertRuleTypeSettings.DataAnalysisDefinitionId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorAPI, dataAnalysisDefinitionSelectorPayload, dataAnalysisDefinitionSelectorLoadDeferred);
                    });
                    promises.push(dataAnalysisDefinitionSelectorLoadDeferred.promise);

                    if (vrAlertRuleTypeSettings != undefined) {
                        var sourceDataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(sourceDataRecordStorageSelectorLoadDeferred.promise);
                        UtilsService.waitMultiplePromises([sourceDataRecordStorageSelectorReadyDeferred.promise, selectedDataAnalysisDefinitionSelectoReadyDeferred.promise]).then(function () {
                            selectedDataAnalysisDefinitionSelectoReadyDeferred = undefined;
                            var filters = [];
                            var daProfCalcDataRecordStorageFilter = {
                                $type: 'Vanrise.Analytic.MainExtensions.DataAnalysis.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.MainExtensions',
                                DataAnalysisDefinitionId: vrAlertRuleTypeSettings.DataAnalysisDefinitionId
                            };
                            filters.push(daProfCalcDataRecordStorageFilter);

                            var recordStoragePayload = {
                                filters: filters,
                                selectedIds: getSelectedSourceRecordStorageIds(vrAlertRuleTypeSettings.SourceRecordStorages)
                            };

                            VRUIUtilsService.callDirectiveLoad(sourceDataRecordStorageSelectorAPI, recordStoragePayload, sourceDataRecordStorageSelectorLoadDeferred);
                        });

                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSettings, Vanrise.Analytic.Entities",
                        DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorAPI.getSelectedIds(),
                        SourceRecordStorages: buildSourceRecordStorages()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };

            function buildSourceRecordStorages() {
                var selectedRecordStorages = [];
                var recordStorageIds = sourceDataRecordStorageSelectorAPI.getSelectedIds();
                for (var x = 0; x < recordStorageIds.length; x++) {
                    var currentItem = recordStorageIds[x];
                    selectedRecordStorages.push({ DataRecordStorageId: currentItem });
                }
                return selectedRecordStorages;
            };

            function getSelectedSourceRecordStorageIds(sourceRecordStorages) {
                if (sourceRecordStorages == undefined)
                    return undefined;

                var selectedRecordStorages = [];
                for (var x = 0; x < sourceRecordStorages.length; x++) {
                    var currentItem = sourceRecordStorages[x];
                    selectedRecordStorages.push(currentItem.DataRecordStorageId);
                }
                return selectedRecordStorages;
            };
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertruletypesettings', DAProfCalcAlertRuleTypeSettings);

})(app);