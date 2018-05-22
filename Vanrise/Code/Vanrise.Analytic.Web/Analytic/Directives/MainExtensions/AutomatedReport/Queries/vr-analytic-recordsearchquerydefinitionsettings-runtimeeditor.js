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
        var dataStorageIsSelectedArrayDeffered = UtilsService.createPromiseDeferred();

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var loadDataRecordStorageSelectorPromise = loadDataRecordStorageSelector();
                var loadTimePeriodSelectorPromise = loadTimePeriodSelector();

                promises.push(loadDataRecordStorageSelectorPromise);
                promises.push(loadTimePeriodSelectorPromise);

                var entity;
                var defintionId;
                var dataStorageArray = [];
                var dataStorageIsSelectedArray = [];
                var dataRecordStoragePayload = [];
                

                if (payload != undefined) {
                    entity = payload.runtimeDirectiveEntity;
                    defintionId = payload.definitionId;
                    
                    if (entity != undefined) {
                        $scope.scopeModel.limitResult = entity.LimitResult;
                        dataRecordStoragePayload = entity.DataRecordStorages;

                        if(dataRecordStoragePayload!=undefined){
                            for (var i = 0; i < dataRecordStoragePayload.length; i++) {
                                dataStorageIsSelectedArray.push(dataRecordStoragePayload[i].DataRecordStorageId);
                            }
                        }
                    }

                    else {
                        $scope.scopeModel.limitResult = '';

                        VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionSettings(defintionId).then(function (response) {
                            dataRecordStorageSelectorAPI.clearDataSource();
                            var arr = response.ExtendedSettings.DataRecordStorages;
                            for (var i = 0; i < arr.length; i++) {
                                dataStorageArray.push(arr[i]);
                            }
                            for (var i = 0; i < dataStorageArray.length; i++) {
                                if (dataStorageArray[i].IsSelected == true) {
                                    dataStorageIsSelectedArray.push(dataStorageArray[i].DataRecordStorageId);
                                }
                            }
                        });
                    }
                    dataStorageIsSelectedArrayDeffered.resolve();
            };
                function loadDataRecordStorageSelector() {
                    var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([dataRecordStorageSelectorReadyDeferred.promise, dataStorageIsSelectedArrayDeffered.promise]).then(function () {
                        var dataStorageSelectorPayload = {
                            selectedIds: defintionId != undefined ? dataStorageIsSelectedArray : undefined
                            };
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                    });
                    return dataRecordStorageSelectorLoadDeferred.promise;
                }

                function loadTimePeriodSelector(){
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodSelectorPayload = {
                            timePeriod: entity != undefined ? entity.TimePeriod : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodSelectorPayload, timePeriodSelectorLoadDeferred);
                    });
                    return timePeriodSelectorLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                var array = dataRecordStorageSelectorAPI.getSelectedIds();
                var dataRecordStoragesArray = [];
                for (var i = 0; i < array.length; i++) {
                    dataRecordStoragesArray.push({ DataRecordStorageId: array [i]});
                }

                return {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQuerySettings,Vanrise.Analytic.MainExtensions',
                    DataRecordStorages: dataRecordStoragesArray,//dataRecordStorageSelectorAPI.getSelectedIds(),//$scope.scopeModel.selectedValues,
                    TimePeriod: timePeriodSelectorAPI.getData(),
                    LimitResult : $scope.scopeModel.limitResult,
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);