(function (app) {

    'use strict';

    DRSourceSubviewDefinitionSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticReportAPIService'];

    function DRSourceSubviewDefinitionSettings(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticReportAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordSourceSubviewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/RecordSearch/SourceSettings/Templates/DataRecordSourceSubviewDefinitionSettingsTemplate.html"
        };

        function DataRecordSourceSubviewDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var subviewDefinitionSettings;
            var currentSourceDataRecordTypeId;
            var selectedSourceDataRecordTypeId;
            var reportEntity;

            var analyticReportSelectorAPI;
            var analyticReportSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onAnalyticReportSelectorSelectionChangedDeferred;

            var onDataRecordSourceSelectorSelectionChangedDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataRecordSources = [];
                $scope.scopeModel.parentSubviewMappings = [];

                $scope.scopeModel.onAnalyticReportSelectorDirectiveReady = function (api) {
                    analyticReportSelectorAPI = api;
                    analyticReportSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAnalyticReportSelectionChanged = function (selectedAnalyticReport) {

                    if (selectedAnalyticReport != undefined) {
                        if (onAnalyticReportSelectorSelectionChangedDeferred != undefined) {
                            onAnalyticReportSelectorSelectionChangedDeferred.resolve();
                        } else {
                            $scope.scopeModel.selectedDataRecordSources = undefined;
                            $scope.scopeModel.dataRecordSources = [];
                            $scope.scopeModel.parentSubviewMappings = [];

                            $scope.scopeModel.isDataRecordSourceSelectorLoading = true;
                            getAnalyticReportDefinition(selectedAnalyticReport.AnalyticReportId).then(function () {
                                $scope.scopeModel.isDataRecordSourceSelectorLoading = false;
                            });
                        }
                    }
                };
                $scope.scopeModel.onDataRecordSourceSelectionChanged = function (selectedDataRecordSource) {

                    if (selectedDataRecordSource != undefined) {
                        var selectedSource = UtilsService.getItemByVal(reportEntity.Settings.Sources, selectedDataRecordSource.Name, 'Name');
                        selectedSourceDataRecordTypeId = selectedSource.DataRecordTypeId;

                        if (onDataRecordSourceSelectorSelectionChangedDeferred != undefined) {
                            onDataRecordSourceSelectorSelectionChangedDeferred.resolve();
                        } else {
                            $scope.scopeModel.parentSubviewMappings = [];
                        }
                    }
                };

                $scope.scopeModel.onAddParentSubviewMapping = function () {
                    $scope.scopeModel.isGridLoading = true;

                    var parentSubviewMapping = {};
                    extendParentSubviewMappingObject(parentSubviewMapping);

                    var promises = [];
                    promises.push(parentSubviewMapping.parentDataRecordFieldSelectorLoadDeferred.promise);
                    promises.push(parentSubviewMapping.subviewDataRecordFieldSelectorLoadDeferred.promise);

                    $scope.scopeModel.parentSubviewMappings.push(parentSubviewMapping);

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                $scope.scopeModel.removeParentSubviewMapping = function (mapping) {

                    var parentDataRecordFieldSelectorAPI = mapping.parentDataRecordFieldSelectorAPI;
                    var parentDataRecordFieldName = parentDataRecordFieldSelectorAPI.getSelectedIds();

                    var subviewDataRecordFieldSelectorAPI = mapping.subviewDataRecordFieldSelectorAPI;
                    var subviewDataRecordFieldName = subviewDataRecordFieldSelectorAPI.getSelectedIds();

                    var index;
                    for (index = 0; index < $scope.scopeModel.parentSubviewMappings.length; index++) {
                        var currentMapping = $scope.scopeModel.parentSubviewMappings[index];

                        var currentParentdataRecordFieldSelectorAPI = currentMapping.parentDataRecordFieldSelectorAPI;
                        var currentParentdataRecordFieldName = currentParentdataRecordFieldSelectorAPI.getSelectedIds();

                        var currentSubviewDataRecordFieldSelectorAPI = currentMapping.subviewDataRecordFieldSelectorAPI;
                        var currentSubviewDataRecordFieldName = currentSubviewDataRecordFieldSelectorAPI.getSelectedIds();

                        if (parentDataRecordFieldName == currentParentdataRecordFieldName && subviewDataRecordFieldName == currentSubviewDataRecordFieldName)
                            break;
                    }

                    $scope.scopeModel.parentSubviewMappings.splice(index, 1);
                };

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.parentSubviewMappings == undefined || $scope.scopeModel.parentSubviewMappings.length == 0)
                        return "you should add one mapping at least";
                    return null;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        subviewDefinitionSettings = payload.subviewDefinitionSettings;
                        currentSourceDataRecordTypeId = payload.dataRecordTypeId;
                    }

                    var analyticReportSelectorLoadPromise = getAnalyticReportSelectorLoadPromise();
                    promises.push(analyticReportSelectorLoadPromise);

                    if (subviewDefinitionSettings) {
                        $scope.scopeModel.includeTimeFilter = subviewDefinitionSettings.IncludeTimeFilter;

                        if (subviewDefinitionSettings.AnalyticReportId) {
                            onAnalyticReportSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                            var dataRecordSourceSelectorLoadPromise = getDataRecordSourceSelectorLoadPromise(subviewDefinitionSettings.AnalyticReportId);
                            promises.push(dataRecordSourceSelectorLoadPromise);
                        }

                        if (subviewDefinitionSettings.Mappings && subviewDefinitionSettings.DRSourceName) {
                            onDataRecordSourceSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                            var gridLoadPromise = getGridLoadPromise();
                            promises.push(gridLoadPromise);
                        }
                    }

                    function getAnalyticReportSelectorLoadPromise() {
                        var analyticReportSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        analyticReportSelectorReadyDeferred.promise.then(function () {

                            var analyticReportSelectorPayLoad = { filter: { TypeName: "VR_Analytic_Report_RecordSearch" } };
                            if (subviewDefinitionSettings && subviewDefinitionSettings.AnalyticReportId) {
                                analyticReportSelectorPayLoad.selectedIds = subviewDefinitionSettings.AnalyticReportId;
                            }
                            VRUIUtilsService.callDirectiveLoad(analyticReportSelectorAPI, analyticReportSelectorPayLoad, analyticReportSelectorLoadDeferred);
                        });

                        return analyticReportSelectorLoadDeferred.promise;
                    }
                    function getDataRecordSourceSelectorLoadPromise(reportId) {
                        var dataRecordSourceSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        onAnalyticReportSelectorSelectionChangedDeferred.promise.then(function () {
                            onAnalyticReportSelectorSelectionChangedDeferred = undefined;

                            getAnalyticReportDefinition(reportId).then(function () {
                                if (subviewDefinitionSettings && subviewDefinitionSettings.DRSourceName)
                                    $scope.scopeModel.selectedDataRecordSources = UtilsService.getItemByVal($scope.scopeModel.dataRecordSources, subviewDefinitionSettings.DRSourceName, 'Name');

                                dataRecordSourceSelectorLoadDeferred.resolve();
                            });
                        });

                        return dataRecordSourceSelectorLoadDeferred.promise;
                    }
                    function getGridLoadPromise() {
                        var gridLoadPromise = UtilsService.createPromiseDeferred();

                        var promises = [];

                        onDataRecordSourceSelectorSelectionChangedDeferred.promise.then(function () {
                            onDataRecordSourceSelectorSelectionChangedDeferred = undefined;

                            for (var index = 0; index < subviewDefinitionSettings.Mappings.length; index++) {
                                var currentItem = subviewDefinitionSettings.Mappings[index];
                                extendParentSubviewMappingObject(currentItem);

                                promises.push(currentItem.parentDataRecordFieldSelectorLoadDeferred.promise);
                                promises.push(currentItem.subviewDataRecordFieldSelectorLoadDeferred.promise);

                                $scope.scopeModel.parentSubviewMappings.push(currentItem);
                            }
                        });

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            gridLoadPromise.resolve();
                        }).catch(function (error) {
                            gridLoadPromise.reject(error);
                        });

                        return gridLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function getData() {

                    var mappings = [];
                    for (var index = 0; index < $scope.scopeModel.parentSubviewMappings.length; index++) {
                        var currentParentSubviewMapping = $scope.scopeModel.parentSubviewMappings[index];
                        var mappingObj = {
                            ParentColumnName: currentParentSubviewMapping.parentDataRecordFieldSelectorAPI.getSelectedIds(),
                            SubviewColumnName: currentParentSubviewMapping.subviewDataRecordFieldSelectorAPI.getSelectedIds()
                        };
                        mappings.push(mappingObj);
                    }

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.DRSearchPageSubviewDefinitions.DRSourceSubviewDefinitionSettings, Vanrise.Analytic.MainExtensions",
                        AnalyticReportId: $scope.scopeModel.selectedAnalyticReport.AnalyticReportId,
                        DRSourceName: $scope.scopeModel.selectedDataRecordSources.Name,
                        IncludeTimeFilter: $scope.scopeModel.includeTimeFilter,
                        Mappings: mappings
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getAnalyticReportDefinition(reportId) {
                return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportById(reportId).then(function (response) {
                    reportEntity = response;

                    if (reportEntity && reportEntity.Settings) {
                        for (var i = 0; i < reportEntity.Settings.Sources.length; i++) {
                            var source = reportEntity.Settings.Sources[i];
                            $scope.scopeModel.dataRecordSources.push({ Name: source.Name, Title: source.Title });
                        }
                    }
                });
            }
            function extendParentSubviewMappingObject(mapping) {

                mapping.parentDataRecordFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                mapping.onParentDataRecordFieldSelectorReady = function (api) {
                    mapping.parentDataRecordFieldSelectorAPI = api;

                    var parentDataRecordFieldPayload = { dataRecordTypeId: currentSourceDataRecordTypeId };
                    if (mapping != undefined) {
                        parentDataRecordFieldPayload.selectedIds = mapping.ParentColumnName;
                    }
                    VRUIUtilsService.callDirectiveLoad(mapping.parentDataRecordFieldSelectorAPI, parentDataRecordFieldPayload, mapping.parentDataRecordFieldSelectorLoadDeferred);
                };

                mapping.subviewDataRecordFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                mapping.onSubviewDataRecordFieldSelectorReady = function (api) {
                    mapping.subviewDataRecordFieldSelectorAPI = api;

                    var subviewDataRecordFieldPayload = { dataRecordTypeId: selectedSourceDataRecordTypeId };
                    if (mapping != undefined) {
                        subviewDataRecordFieldPayload.selectedIds = mapping.SubviewColumnName;
                    }
                    VRUIUtilsService.callDirectiveLoad(mapping.subviewDataRecordFieldSelectorAPI, subviewDataRecordFieldPayload, mapping.subviewDataRecordFieldSelectorLoadDeferred);
                };
            }
        }
    }

    app.directive('vrAnalyticDatarecordsourceSubviewdefinitionsettings', DRSourceSubviewDefinitionSettings);

})(app);