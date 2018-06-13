"use strict";
app.directive("vrRecordsearchquerydefinitionAutomatedreport", ["UtilsService","VR_GenericData_DataRecordStorageAPIService", "VRUIUtilsService",
function (UtilsService,VR_GenericData_DataRecordStorageAPIService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var recordSearchQuery = new RecordSearchQuery($scope, ctrl, $attrs);
            recordSearchQuery.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/RecordSearchQueryDefinition.html"
    };


    function RecordSearchQuery($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeSelectedPromiseDeferred;

        var dataRecordTypeId;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.columns = [];

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

           
            $scope.scopeModel.onGridReady = function (api) {
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            };

            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {
                    if (dataRecordTypeSelectedPromiseDeferred != undefined) {
                        dataRecordTypeSelectedPromiseDeferred.resolve();
                    }
                    else {
                        $scope.scopeModel.columns.length = 0;
                        var dataRecordTypeSelectedId = selectedDataRecordType.DataRecordTypeId;
                        if (dataRecordTypeSelectedId != undefined) {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingDataStorageSelector = value;
                            };
                            var dataRecordStoragePayload = {
                                DataRecordTypeId: dataRecordTypeSelectedId
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordStorageSelectorAPI, dataRecordStoragePayload, setLoader, dataRecordTypeSelectedPromiseDeferred);
                        }
                    }
                }
                
            };

            $scope.scopeModel.addColumn = function () {
                var gridItem = {
                    id: $scope.scopeModel.columns.length + 1,
                    columnId: $scope.scopeModel.selectedDataRecordStorage.DataRecordStorageId,
                    columnName: $scope.scopeModel.selectedDataRecordStorage.Name,
                    isSelected: true,
                };
                $scope.scopeModel.columns.push(gridItem);
                $scope.addButton = true;
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                if (index > -1) {
                    $scope.scopeModel.columns.splice(index, 1);
                }
            };

            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.columns.length == 0) {
                    return 'At least one record must be added.';
                }
                var columnIds = [];
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    if ($scope.scopeModel.columns[i].columnId != undefined) {
                        columnIds.push($scope.scopeModel.columns[i].columnId.toUpperCase());
                    }
                }
                while (columnIds.length > 0) {
                    var nameToValidate = columnIds[0];
                    columnIds.splice(0, 1);
                    if (!validateName(nameToValidate, columnIds)) {
                        return 'Two or more columns have the same name.';
                    }
                }
                return null;
                function validateName(name, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j] == name)
                            return false;
                    }
                    return true;
                }
            };
        }

        function getDirectiveAPI() {

            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.extendedSettings!=undefined) {
                    dataRecordTypeId = payload.extendedSettings.DataRecordTypeId;
                    dataRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                }
                var promises = [];

                var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
                promises.push(loadDataRecordTypeSelectorPromise);
                
                if (dataRecordTypeId != undefined) {
                    var loadDataRecordStorageSelectorPromise = loadDataRecordStorageSelector();
                    promises.push(loadDataRecordStorageSelectorPromise);
                }

                function loadDataRecordTypeSelector() {
                    var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                        var payload = {
                            selectedIds: dataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
                    });
                    return dataRecordTypeSelectorLoadDeferred.promise;
                }

                function loadDataRecordStorageSelector() {
                    var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    if (payload != undefined) {
                        var dataRecordStorages = payload.extendedSettings.DataRecordStorages;
                        var dataRecordStorageIdsList = [];
                       
                        for (var i = 0; i < dataRecordStorages.length; i++) {
                            dataRecordStorageIdsList.push(dataRecordStorages[i].DataRecordStorageId);
                        }

                        VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorageList(dataRecordStorageIdsList).then(function (response) {
                            for (var i = 0; i < dataRecordStorages.length; i++) {
                                var dataRecordStorage = dataRecordStorages[i];
                                var gridItem = {
                                    id: i,
                                    columnId: dataRecordStorage.DataRecordStorageId,
                                    columnName: getName(),
                                    isSelected: dataRecordStorage.IsSelected,
                                };

                                function getName() {
                                    for (var j = 0; j < response.length; j++) {
                                        if (response[j].DataRecordStorageId == dataRecordStorage.DataRecordStorageId) {
                                            return response[j].Name;
                                        }
                                    }
                                }
                                $scope.scopeModel.columns.push(gridItem);
                            }
                        });
                    }
                    UtilsService.waitMultiplePromises([dataRecordStorageSelectorReadyDeferred.promise, dataRecordTypeSelectedPromiseDeferred.promise]).then(function () {
                        dataRecordTypeSelectedPromiseDeferred = undefined;
                        var dataRecordStoragePayload = {
                            DataRecordTypeId : dataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataRecordStoragePayload, dataRecordStorageSelectorLoadDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                }
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQueryDefinitionSettings,Vanrise.Analytic.MainExtensions",
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    DataRecordStorages: getColumns()
                };

                function getColumns() {
                    var columns;
                    if ($scope.scopeModel.columns.length > 0) {
                        columns = [];
                        for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                            var column = $scope.scopeModel.columns[i];
                            columns.push({
                                DataRecordStorageId: column.columnId,
                                IsSelected: column.isSelected,
                            });
                        }
                    }
                    return columns;
                }
            };

            return api;
        }
    }

    return directiveDefinitionObject;
}
]);