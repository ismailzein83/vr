'use strict';

app.directive('vrGenericdataOnnotificationcreatedactionSaverecord', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_Notification_NotificationActionMappingFieldEnum', 'VR_GenericData_DataRecordStorageAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_Notification_NotificationActionMappingFieldEnum, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OnNotificationCreatedSaveRecord($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Notification/Directive/Templates/SaveDataRecordNotificationActionTemplate.html'
        };

        function OnNotificationCreatedSaveRecord($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFields = [];
            var dataRecordTypeId;
            var mappingDataRecordTypeId;

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordStorageSelectionChangedPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataRecordTypeFields = [];
                $scope.scopeModel.mappingFieldTypes = [];
                $scope.scopeModel.mappingDataRecordTypeFields = [];

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordStorageSelectionChanged = function (selectedRecordStorage) {
                    if (selectedRecordStorage == undefined)
                        return;

                    if (dataRecordStorageSelectionChangedPromiseDeferred != undefined) {
                        dataRecordStorageSelectionChangedPromiseDeferred.resolve();
                        dataRecordStorageSelectionChangedPromiseDeferred = undefined;
                    }
                    else {
                        dataRecordTypeFields.length = 0;
                        $scope.scopeModel.dataRecordTypeFields.length = 0;
                        $scope.scopeModel.isGridLoading = true;

                        var getDataRecordFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

                        if (selectedRecordStorage != undefined && selectedRecordStorage.DataRecordTypeId != undefined) {
                            dataRecordTypeId = selectedRecordStorage.DataRecordTypeId;
                            VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                                if (response != undefined)
                                    for (var i = 0; i < response.length; i++) {
                                        var currentField = response[i];
                                        $scope.scopeModel.dataRecordTypeFields.push(currentField.Entity);
                                    }

                                getDataRecordFieldsPromiseDeferred.resolve();
                            });
                        }
                        else {
                            getDataRecordFieldsPromiseDeferred.resolve();
                        }

                        getDataRecordFieldsPromiseDeferred.promise.then(function () {
                            $scope.scopeModel.isGridLoading = false;
                        });
                    }

                };

                $scope.scopeModel.validateGrid = function () {
                    if ($scope.scopeModel.dataRecordTypeFields.length == 0)
                        return "No Record Fields for Mapping";

                    var gridItemsMapping = [];
                    for (var i = 0; i < $scope.scopeModel.dataRecordTypeFields.length; i++) {
                        var dataRecordTypeField = $scope.scopeModel.dataRecordTypeFields[i];
                        if (dataRecordTypeField.selectedMappingField == undefined)
                            continue;

                        var gridItem = {
                            RecordFieldName: dataRecordTypeField.Name,
                            MappingFieldType: dataRecordTypeField.selectedMappingField.value
                        };

                        if (dataRecordTypeField.selectedMappingField.showRecordFieldsSelector && dataRecordTypeField.selectedDataRecordField != undefined)
                            gridItem.MappingFieldName = dataRecordTypeField.selectedDataRecordField.Name;

                        gridItemsMapping.push(gridItem);
                    }

                    if (gridItemsMapping.length == 0)
                        return "At least one Field should be Mapped";

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    var dataRecordStorageId;
                    var gridItemsMapping;
                    $scope.scopeModel.mappingFieldTypes = UtilsService.getArrayEnum(VR_Notification_NotificationActionMappingFieldEnum);

                    if (payload != undefined) {
                        mappingDataRecordTypeId = payload.mappingDataRecordTypeId;
                        dataRecordTypeId = payload.DataRecordTypeId;
                        dataRecordStorageId = payload.DataRecordStorageId;
                        gridItemsMapping = payload.GridItemsMapping;
                    }

                    if (dataRecordStorageId != undefined) {
                        dataRecordStorageSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();
                        initialPromises.push(dataRecordStorageSelectionChangedPromiseDeferred.promise);

                        var getDataRecordFieldsPromise = getDataRecordFields();
                        initialPromises.push(getDataRecordFieldsPromise);


                    }

                    if (dataRecordTypeId != undefined) {

                    }

                    if (mappingDataRecordTypeId != undefined) {
                        var getMappingDataRecordFieldsPromise = getMappingDataRecordFields();
                        initialPromises.push(getMappingDataRecordFieldsPromise);
                    }

                    var dataRecordStorageSelectorLoadPromise = loadDataRecordStorageSelector();
                    initialPromises.push(dataRecordStorageSelectorLoadPromise);

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var childPromises = [];

                            if (dataRecordTypeFields.length > 0 && gridItemsMapping != undefined && gridItemsMapping.length > 0) {
                                childPromises.push(loadGridItems());
                            }
                            else {
                                $scope.scopeModel.dataRecordTypeFields = dataRecordTypeFields;
                            }

                            return {
                                promises: childPromises
                            };
                        }
                    };

                    function loadDataRecordStorageSelector() {
                        var dataRecordStorageSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordStorageSelectorPromiseDeferred.promise.then(function () {
                            var dataRecordStorageSelectorPayload;
                            if (dataRecordStorageId != undefined) {
                                dataRecordStorageSelectorPayload = {
                                    selectedIds: dataRecordStorageId
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataRecordStorageSelectorPayload, dataRecordStorageSelectorLoadPromiseDeferred);
                        });

                        return dataRecordStorageSelectorLoadPromiseDeferred.promise;
                    }

                    function getMappingDataRecordFields() {
                        var getMappingDataRecordFieldsPromiseDeferred = UtilsService.createPromiseDeferred();
                        VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(mappingDataRecordTypeId).then(function (response) {
                            if (response != undefined)
                                for (var i = 0; i < response.length; i++) {
                                    var currentField = response[i].Entity;
                                    $scope.scopeModel.mappingDataRecordTypeFields.push(currentField);
                                }

                            getMappingDataRecordFieldsPromiseDeferred.resolve();
                        });

                        return getMappingDataRecordFieldsPromiseDeferred.promise;
                    }

                    function getDataRecordFields() {
                        var getDataRecordFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

                        VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataRecordStorageId).then(function (response) {
                            if (response) {
                                dataRecordTypeId = response.DataRecordTypeId;
                                VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                                    if (response != undefined)
                                        for (var i = 0; i < response.length; i++) {
                                            var currentField = response[i].Entity;
                                            dataRecordTypeFields.push(currentField);
                                        }

                                    getDataRecordFieldsPromiseDeferred.resolve();
                                });

                            }
                        });


                        return getDataRecordFieldsPromiseDeferred.promise;
                    }

                    function loadGridItems() {
                        var loadGridItemsPromiseDeferred = UtilsService.createPromiseDeferred();

                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            var dataRecordField = UtilsService.cloneObject(dataRecordTypeFields[i]);

                            var currentGridItem = UtilsService.getItemByVal(gridItemsMapping, dataRecordField.Name, "RecordFieldName");
                            if (currentGridItem == undefined) {
                                $scope.scopeModel.dataRecordTypeFields.push(dataRecordField);
                                continue;
                            }

                            dataRecordField.selectedMappingField = UtilsService.getEnum(VR_Notification_NotificationActionMappingFieldEnum, "value", currentGridItem.MappingFieldType);
                            dataRecordField.selectedDataRecordField = UtilsService.getItemByVal($scope.scopeModel.mappingDataRecordTypeFields, currentGridItem.MappingFieldName, "Name");

                            $scope.scopeModel.dataRecordTypeFields.push(dataRecordField);
                        }

                        loadGridItemsPromiseDeferred.resolve();

                        return loadGridItemsPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        //dataRecordStorageSelectionChangedPromiseDeferred = undefined;
                    });
                };

                api.getData = function () {
                    function getGridItemsMapping() {
                        if ($scope.scopeModel.dataRecordTypeFields.length == 0)
                            return undefined;

                        var gridItemsMapping = [];
                        for (var i = 0; i < $scope.scopeModel.dataRecordTypeFields.length; i++) {
                            var dataRecordTypeField = $scope.scopeModel.dataRecordTypeFields[i];
                            if (dataRecordTypeField.selectedMappingField == undefined)
                                continue;

                            var gridItem = {
                                RecordFieldName: dataRecordTypeField.Name,
                                MappingFieldType: dataRecordTypeField.selectedMappingField.value
                            };

                            if (dataRecordTypeField.selectedMappingField.showRecordFieldsSelector && dataRecordTypeField.selectedDataRecordField != undefined)
                                gridItem.MappingFieldName = dataRecordTypeField.selectedDataRecordField.Name;

                            gridItemsMapping.push(gridItem);
                        }

                        return gridItemsMapping.length > 0 ? gridItemsMapping : undefined;
                    }

                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.VRActions.OnDataRecordNotificationCreatedSaveAction, Vanrise.GenericData.MainExtensions',
                        DataRecordTypeId: dataRecordTypeId,
                        DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds(),
                        GridItemsMapping: getGridItemsMapping()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);