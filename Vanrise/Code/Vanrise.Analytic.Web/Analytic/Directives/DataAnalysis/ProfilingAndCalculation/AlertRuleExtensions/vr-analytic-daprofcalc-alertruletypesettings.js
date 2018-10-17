(function (app) {

    'use strict';

    DAProfCalcAlertRuleTypeSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_Analytic_DataAnalysisItemDefinitionAPIService'];

    function DAProfCalcAlertRuleTypeSettings(UtilsService, VRUIUtilsService, VRNotificationService, VR_Analytic_DataAnalysisItemDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
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
            var dataAnalysisDefinitionSelectionChangedDeferred;

            var sourceDataRecordStorageSelectorAPI;
            var sourceDataRecordStorageSelectorReadyDeferred;

            var daProfCalcAlertRuleFilterDefinitionDirectiveAPI;
            var daProfCalcAlertRuleFilterDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var daProfCalcGridAPI;

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var startInstancePermissionAPI;
            var startInstancePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.daProfCalcItemNotifications = [];

                $scope.scopeModel.onDataAnalysisDefinitionSelectorReady = function (api) {
                    dataAnalysisDefinitionSelectorAPI = api;
                    dataAnalysisDefinitionSelectoReadyDeferred.resolve();
                };
                $scope.scopeModel.onSourceDataRecordStorageSelectorReady = function (api) {
                    sourceDataRecordStorageSelectorAPI = api;

                    var filters = [];
                    var daProfCalcDataRecordStorageFilter = {
                        $type: 'Vanrise.Analytic.Business.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.Business',
                        DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorAPI.getSelectedIds()
                    };
                    filters.push(daProfCalcDataRecordStorageFilter);

                    var payload = {
                        filters: filters
                    };
                    var setSourceLoader = function (value) {
                        $scope.scopeModel.isLoadingSourceDataRecordStorage = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceDataRecordStorageSelectorAPI, payload, setSourceLoader, sourceDataRecordStorageSelectorReadyDeferred);
                };

                $scope.scopeModel.onDAProfCalcAlertRuleFilterDefinitionReady = function (api) {
                    daProfCalcAlertRuleFilterDefinitionDirectiveAPI = api;
                    daProfCalcAlertRuleFilterDefinitionDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDAProfCalcGridReady = function (api) {
                    daProfCalcGridAPI = api;
                };
                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onStartInstanceRequiredPermissionReady = function (api) {
                    startInstancePermissionAPI = api;
                    startInstancePermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalysisDefinitionSelectionChanged = function (dataItem) {
                    if (dataItem != undefined) {
                        if (sourceDataRecordStorageSelectorAPI != undefined) {
                            var filters = [];
                            var daProfCalcDataRecordStorageFilter = {
                                $type: 'Vanrise.Analytic.Business.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.Business',
                                DataAnalysisDefinitionId: dataItem.DataAnalysisDefinitionId
                            };
                            filters.push(daProfCalcDataRecordStorageFilter);

                            var payload = {
                                filters: filters
                            };

                            var setSourceLoader = function (value) {
                                $scope.scopeModel.isLoadingSourceDataRecordStorage = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceDataRecordStorageSelectorAPI, payload, setSourceLoader, sourceDataRecordStorageSelectorReadyDeferred);
                        }

                        if (dataAnalysisDefinitionSelectionChangedDeferred != undefined) {
                            dataAnalysisDefinitionSelectionChangedDeferred.resolve();
                        }
                        else {
                            var filters = [];
                            var daProfCalcDataAnalysisItemDefinitionFilter = {
                                $type: "Vanrise.Analytic.Entities.DAProfCalcDataAnalysisItemDefinitionFilter, Vanrise.Analytic.Entities"
                            };
                            filters.push(daProfCalcDataAnalysisItemDefinitionFilter);

                            var filter = {
                                Filters: filters
                            };

                            $scope.scopeModel.daProfCalcItemNotifications.length = 0;
                            $scope.scopeModel.isLoadingGrid = true;

                            var serializedFilter = UtilsService.serializetoJson(filter) != undefined ? UtilsService.serializetoJson(filter) : {};

                            var dataAnalysisItemPromise = VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinitionsInfo(serializedFilter, dataItem.DataAnalysisDefinitionId).then(function (response) {
                                $scope.scopeModel.selectedDataAnalysisItems = response;

                                var _promises = [];

                                if ($scope.scopeModel.selectedDataAnalysisItems != null) {
                                    for (var i = 0; i < $scope.scopeModel.selectedDataAnalysisItems.length; i++) {
                                        var selectedDataAnalysisItem = $scope.scopeModel.selectedDataAnalysisItems[i];
                                        var dataAnalysisNotificationItem = {
                                            DataAnalysisItemDefinitionId: selectedDataAnalysisItem.DataAnalysisItemDefinitionId,
                                            Name: selectedDataAnalysisItem.Name
                                        };
                                        $scope.scopeModel.daProfCalcItemNotifications.push(dataAnalysisNotificationItem);
                                        _promises.push(extendDataAnalysisNotificationItemObject(dataAnalysisNotificationItem, undefined));
                                    }
                                }

                                UtilsService.waitMultiplePromises(_promises).then(function () {
                                    $scope.scopeModel.isLoadingGrid = false;
                                });
                            });
                        }
                    }
                };

                defineAPI();
            };
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrAlertRuleTypeSettings;

                    if (payload != undefined)
                        vrAlertRuleTypeSettings = payload.settings;

                    if (vrAlertRuleTypeSettings != undefined) {
                        $scope.scopeModel.rawRecordFilterLabel = vrAlertRuleTypeSettings.RawRecordFilterLabel;
                        dataAnalysisDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        promises.push(dataAnalysisDefinitionSelectionChangedDeferred.promise);
                    }

                    //Loading DataAnalysisDefinition Selector
                    var loadDataAnalysisDefinitionSelectorPromise = loadDataAnalysisDefinitionSelector();
                    promises.push(loadDataAnalysisDefinitionSelectorPromise);

                    //Loading DAProfCalcAlertRuleFilterDefinition Directive
                    var loadDAProfCalcAlertRuleFilterDefinitionDirectivePromise = loadDAProfCalcAlertRuleFilterDefinitionDirective();
                    promises.push(loadDAProfCalcAlertRuleFilterDefinitionDirectivePromise);

                    if (vrAlertRuleTypeSettings != undefined) {
                        var filters = [];
                        var daProfCalcDataAnalysisItemDefinitionFilter = {
                            $type: "Vanrise.Analytic.Entities.DAProfCalcDataAnalysisItemDefinitionFilter, Vanrise.Analytic.Entities"
                        };
                        filters.push(daProfCalcDataAnalysisItemDefinitionFilter);
                        var filter = {
                            Filters: filters
                        };
                        var serializedFilter = UtilsService.serializetoJson(filter) != undefined ? UtilsService.serializetoJson(filter) : {};

                        var dataAnalysisItemPromise = VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinitionsInfo(serializedFilter, vrAlertRuleTypeSettings.DataAnalysisDefinitionId).then(function (response) {
                            $scope.scopeModel.selectedDataAnalysisItems = response;
                        });
                        promises.push(dataAnalysisItemPromise);

                        UtilsService.waitMultiplePromises([dataAnalysisItemPromise, dataAnalysisDefinitionSelectionChangedDeferred.promise]).then(function () {
                            dataAnalysisDefinitionSelectionChangedDeferred = undefined;

                            if ($scope.scopeModel.selectedDataAnalysisItems != null) {
                                for (var i = 0; i < $scope.scopeModel.selectedDataAnalysisItems.length; i++) {
                                    var selectedDataAnalysisItem = $scope.scopeModel.selectedDataAnalysisItems[i];

                                    var dataAnalysisNotificationItem = {
                                        DataAnalysisItemDefinitionId: selectedDataAnalysisItem.DataAnalysisItemDefinitionId,
                                        Name: selectedDataAnalysisItem.Name
                                    };

                                    var daProfCalcItemNotification;
                                    if (vrAlertRuleTypeSettings.DAProfCalcItemNotifications != undefined) {
                                        daProfCalcItemNotification = UtilsService.getItemByVal(vrAlertRuleTypeSettings.DAProfCalcItemNotifications, selectedDataAnalysisItem.DataAnalysisItemDefinitionId, 'DataAnalysisItemDefinitionId');
                                    }

                                    $scope.scopeModel.daProfCalcItemNotifications.push(dataAnalysisNotificationItem);
                                    promises.push(extendDataAnalysisNotificationItemObject(dataAnalysisNotificationItem, daProfCalcItemNotification));
                                }
                            }
                        });

                        sourceDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

                        var sourceDataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        sourceDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                            sourceDataRecordStorageSelectorReadyDeferred = undefined;

                            var filters = [];
                            var daProfCalcDataRecordStorageFilter = {
                                $type: 'Vanrise.Analytic.Business.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.Business',
                                DataAnalysisDefinitionId: vrAlertRuleTypeSettings.DataAnalysisDefinitionId
                            };
                            filters.push(daProfCalcDataRecordStorageFilter);

                            var recordStoragePayload = {
                                filters: filters,
                                selectedIds: getSelectedSourceRecordStorageIds(vrAlertRuleTypeSettings.SourceRecordStorages)
                            };
                            VRUIUtilsService.callDirectiveLoad(sourceDataRecordStorageSelectorAPI, recordStoragePayload, sourceDataRecordStorageSelectorLoadDeferred);
                        });
                        promises.push(sourceDataRecordStorageSelectorLoadDeferred.promise);

                        var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                        promises.push(loadViewRequiredPermissionPromise);

                        var loadStartInstanceRequiredPermissionPromise = loadStartInstanceRequiredPermission();
                        promises.push(loadStartInstanceRequiredPermissionPromise);
                    }

                    function loadDataAnalysisDefinitionSelector() {
                        var dataAnalysisDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataAnalysisDefinitionSelectoReadyDeferred.promise.then(function () {

                            var dataAnalysisDefinitionSelectorPayload;
                            if (vrAlertRuleTypeSettings != undefined) {
                                dataAnalysisDefinitionSelectorPayload = { selectedIds: vrAlertRuleTypeSettings.DataAnalysisDefinitionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorAPI, dataAnalysisDefinitionSelectorPayload, dataAnalysisDefinitionSelectorLoadDeferred);
                        });

                        return dataAnalysisDefinitionSelectorLoadDeferred.promise;
                    }
                    function loadDAProfCalcAlertRuleFilterDefinitionDirective() {
                        var daProfCalcAlertRuleFilterDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                        daProfCalcAlertRuleFilterDefinitionDirectiveReadyDeferred.promise.then(function () {

                            var daProfCalcAlertRuleFilterDefinitionPayload;
                            if (vrAlertRuleTypeSettings != undefined) {
                                daProfCalcAlertRuleFilterDefinitionPayload = {
                                    daProfCalcAlertRuleFilterDefinition: vrAlertRuleTypeSettings.DAProfCalcAlertRuleFilterDefinition
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(daProfCalcAlertRuleFilterDefinitionDirectiveAPI, daProfCalcAlertRuleFilterDefinitionPayload, daProfCalcAlertRuleFilterDefinitionLoadDeferred);
                        });

                        return daProfCalcAlertRuleFilterDefinitionLoadDeferred.promise;
                    }
                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: vrAlertRuleTypeSettings && vrAlertRuleTypeSettings.DAProfCalcSecurity && vrAlertRuleTypeSettings.DAProfCalcSecurity.ViewPermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });

                        return viewSettingPermissionLoadDeferred.promise;
                    }
                    function loadStartInstanceRequiredPermission() {
                        var startInstancePermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        startInstancePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: vrAlertRuleTypeSettings && vrAlertRuleTypeSettings.DAProfCalcSecurity && vrAlertRuleTypeSettings.DAProfCalcSecurity.StartInstancePermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(startInstancePermissionAPI, dataPayload, startInstancePermissionLoadDeferred);
                        });

                        return startInstancePermissionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSettings, Vanrise.Analytic.Entities",
                        RawRecordFilterLabel: $scope.scopeModel.rawRecordFilterLabel,
                        DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorAPI.getSelectedIds(),
                        SourceRecordStorages: buildSourceRecordStorages(),
                        DAProfCalcItemNotifications: buildDAProfCalcItemNotifications(),
                        DAProfCalcAlertRuleFilterDefinition: daProfCalcAlertRuleFilterDefinitionDirectiveAPI.getData(),
                        DAProfCalcSecurity: {
                            ViewPermission: viewPermissionAPI.getData(),
                            StartInstancePermission: startInstancePermissionAPI.getData()
                        }
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };

            function extendDataAnalysisNotificationItemObject(dataAnalysisNotificationItem, daProfCalcItemNotification) {
                dataAnalysisNotificationItem.dataRecordFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                dataAnalysisNotificationItem.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    dataAnalysisNotificationItem.dataRecordFieldSelectorAPI = api;

                    var filters = [];
                    var daProfCalcNotificationTypeFilter = {
                        $type: "Vanrise.Analytic.Business.DAProfCalcNotificationTypeFilter, Vanrise.Analytic.Business",
                        DataAnalysisItemDefinitionId: dataAnalysisNotificationItem.DataAnalysisItemDefinitionId
                    };
                    filters.push(daProfCalcNotificationTypeFilter);
                    var filter = {
                        Filters: filters
                    };
                    var dataAnalysisNotificationPayload = {
                        filter: filter
                    };

                    if (daProfCalcItemNotification != undefined) {
                        dataAnalysisNotificationPayload.selectedIds = daProfCalcItemNotification.NotificationTypeId;
                    }
                    VRUIUtilsService.callDirectiveLoad(dataAnalysisNotificationItem.dataRecordFieldSelectorAPI, dataAnalysisNotificationPayload, dataAnalysisNotificationItem.dataRecordFieldSelectorLoadDeferred);
                };
                return dataAnalysisNotificationItem.dataRecordFieldSelectorLoadDeferred.promise;
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

            function buildDAProfCalcItemNotifications() {
                var selectedDAProfCalcItemNotifications = [];
                for (var x = 0; x < $scope.scopeModel.daProfCalcItemNotifications.length; x++) {
                    var currentItem = $scope.scopeModel.daProfCalcItemNotifications[x];
                    selectedDAProfCalcItemNotifications.push({ DataAnalysisItemDefinitionId: currentItem.DataAnalysisItemDefinitionId, NotificationTypeId: currentItem.dataRecordFieldSelectorAPI.getSelectedIds() });
                }
                return selectedDAProfCalcItemNotifications;
            };
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertruletypesettings', DAProfCalcAlertRuleTypeSettings);

})(app);