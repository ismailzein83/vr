"use strict";
app.directive("vrAnalyticRecordsearchquerydefinitionsettingsRuntimeeditor", ["VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService", "UtilsService", "VRUIUtilsService",
function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new RuntimeEditor($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: "ctrlrutnime",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/RecordSearchQueryDefinitionSettingsRuntimeEditorTemplate.html"
    };

    function RuntimeEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
       
        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;
                timePeriodSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var dataStorageIsSelectedArrayDeffered = UtilsService.createPromiseDeferred();
                var dataRecordTypeIdDeferred = UtilsService.createPromiseDeferred();
                var promises = [];
                var entity;
                var definitionId;
                var dataStorageArray = [];
                var dataStorageIsSelectedArray = [];
                var dataRecordStoragePayload = [];
                var columns = [];
                var columnNames = [];
                var context;

                dataRecordTypeId = undefined;
                if (payload != undefined) {
                    entity = payload.runtimeDirectiveEntity;
                    definitionId = payload.definitionId;
                    context = payload.context;
                    if (entity != undefined) {
                        $scope.scopeModel.limitResult = entity.LimitResult;
                        dataRecordStoragePayload = entity.DataRecordStorages;
                        columns = entity.Columns;
                        if (dataRecordStoragePayload != undefined) {
                            for (var i = 0; i < dataRecordStoragePayload.length; i++) {
                                dataStorageIsSelectedArray.push(dataRecordStoragePayload[i].DataRecordStorageId);
                            }
                            dataStorageIsSelectedArrayDeffered.resolve();
                        }
                        if (columns != undefined) {
                            for (var i = 0; i < columns.length; i++) {
                                var column = columns[i];
                                if (column != undefined) {
                                    columnNames.push(column.ColumnName);
                                }
                            }
                        }
                    }

                    VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionSettings(definitionId).then(function (response) {
                        if (response != undefined && response.ExtendedSettings != undefined) {
                            if (entity == undefined) {
                                dataRecordStorageSelectorAPI.clearDataSource();
                                $scope.scopeModel.limitResult = '';
                                var arr = response.ExtendedSettings.DataRecordStorages;
                                for (var i = 0; i < arr.length; i++)
                                {
                                    dataStorageArray.push(arr[i]);
                                }
                                for (var i = 0; i < dataStorageArray.length; i++)
                                {
                                    if (dataStorageArray[i].IsSelected == true) {
                                        dataStorageIsSelectedArray.push(dataStorageArray[i].DataRecordStorageId);
                                    }
                                }
                                dataStorageIsSelectedArrayDeffered.resolve();
                            }
                            dataRecordTypeId = response.ExtendedSettings.DataRecordTypeId;
                            dataRecordTypeIdDeferred.resolve();
                        }
                    });
                    
                };

                var loadDataRecordStorageSelectorPromise = loadDataRecordStorageSelector();
                var loadTimePeriodSelectorPromise = loadTimePeriodSelector();
                var loadDataRecordTypeFieldsSelectorPromise = loadDataRecordTypeFieldsSelector();

                promises.push(loadDataRecordStorageSelectorPromise);
                promises.push(loadTimePeriodSelectorPromise);
                promises.push(loadDataRecordTypeFieldsSelectorPromise);

                function loadDataRecordStorageSelector() {
                    var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var promises = [dataRecordStorageSelectorReadyDeferred.promise, dataStorageIsSelectedArrayDeffered.promise, dataRecordTypeIdDeferred.promise];
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        var dataStorageSelectorPayload = {
                            DataRecordTypeId: definitionId != undefined ? dataRecordTypeId : undefined,
                            selectedIds: definitionId != undefined ? dataStorageIsSelectedArray : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                    });
                    return dataRecordStorageSelectorLoadDeferred.promise;
                }

                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodSelectorPayload = {
                            timePeriod: entity != undefined ? entity.TimePeriod : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodSelectorPayload, timePeriodSelectorLoadDeferred);
                    });
                    return timePeriodSelectorLoadDeferred.promise;
                }

                function loadDataRecordTypeFieldsSelector() {
                    var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    var promises = [dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise, dataRecordTypeIdDeferred.promise];
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        var typeFieldsPayload = {
                            dataRecordTypeId: dataRecordTypeId,
                            selectedIds: columnNames
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                    });
                    return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                var array = dataRecordStorageSelectorAPI.getSelectedIds();
                var dataRecordStoragesArray = [];
                for (var i = 0; i < array.length; i++) {
                    dataRecordStoragesArray.push({ DataRecordStorageId: array [i]});
                }

                var dataRecordTypeFields = dataRecordTypeFieldsSelectorAPI.getSelectedIds();
                var columnNames =[];
                var columnsArray = [];

                for (var i = 0; i < $scope.scopeModel.selectedFields.length; i++) {
                    var field = $scope.scopeModel.selectedFields[i];
                    if (field != undefined) {
                        columnsArray.push({
                            ColumnName: field.Name,
                            ColumnTitle: field.Title
                        });
                        columnNames.push(field.Name);
                    }
                }

                return {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQuerySettings,Vanrise.Analytic.MainExtensions',
                    DataRecordStorages: dataRecordStoragesArray,
                    TimePeriod: timePeriodSelectorAPI.getData(),
                    LimitResult: $scope.scopeModel.limitResult,
                    Columns: columnsArray
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {
            };
            return currentContext;
        }
    }

    return directiveDefinitionObject;
}
]);