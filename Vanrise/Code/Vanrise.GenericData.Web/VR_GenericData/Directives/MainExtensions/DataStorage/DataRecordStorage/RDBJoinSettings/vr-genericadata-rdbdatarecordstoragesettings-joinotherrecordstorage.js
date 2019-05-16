'use strict';

app.directive('vrGenericadataRdbdatarecordstoragesettingsJoinotherrecordstorage', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_RDBJoinTypeEnum', 'VR_GenericData_DataRecordStorageAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_RDBJoinTypeEnum, VR_GenericData_DataRecordStorageAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new JoinOtherRecordStorageCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/JoinOtherRecordStorageTemplate.html"
        };

        function JoinOtherRecordStorageCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var context;
            var joinsList = [];
            var joinSettings;
            var recordStorageId;
            var joinType;
            var joinConditions = [];
            var currentDataRecordStorageEntity;

            var dataRecordStorageAPI;
            var dataRecordStorageReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedRecordStorageSelectionChangeDeffered;

            var joinTypeSelectorAPI;
            var joinTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var joinConditionsGridAPI;
            var joinConditionsGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.joinTypes = [];
                $scope.scopeModel.joinConditions = [];
                $scope.scopeModel.sourceStorageJoinNames = [];

                $scope.scopeModel.disableAddRDBJoin = function () {
                    return $scope.scopeModel.selectedDataRecordStorages != undefined ? false : true;
                };

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageAPI = api;
                    dataRecordStorageReadyDeferred.resolve();
                };

                $scope.scopeModel.onJoinTypeSelectorReady = function (api) {
                    joinTypeSelectorAPI = api;
                    joinTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onJoinConditionsGridReady = function (api) {
                    joinConditionsGridAPI = api;
                    joinConditionsGridReadyDeferred.resolve();
                };

                $scope.scopeModel.addJoinCondition = function () {
                    addJoinCondition();
                };

                $scope.scopeModel.onDataRecordStorageSelectionChanged = function (selctedDataRecordStorage) {
                    if (selctedDataRecordStorage != undefined) {
                        currentDataRecordStorageEntity = selctedDataRecordStorage;

                        if (selectedRecordStorageSelectionChangeDeffered != undefined) {
                            selectedRecordStorageSelectionChangeDeffered.resolve();
                        }
                        else {
                            var joinConditionsList = $scope.scopeModel.joinConditions;
                            var joinConditionsListLength = joinConditionsList.length;

                            for (var i = 0; i < joinConditionsListLength; i++) {
                                var dataItem = joinConditionsList[i];
                                var setLoader = function (value) {
                                    $scope.scopeModel.isGridLoading = value;
                                };
                                var storageToJoinDataRecordTypeFieldSelectorPayload = {
                                    dataRecordTypeId: currentDataRecordStorageEntity.DataRecordTypeId,
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.storageToJoinFieldNameSelctorAPI, storageToJoinDataRecordTypeFieldSelectorPayload, setLoader);
                            }
                        }
                    }
                };

                $scope.scopeModel.removeJoinConditions = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.joinConditions, dataItem.sourceStorageJoinNames, 'sourceStorageJoinNames');
                    if (index > -1) {
                        $scope.scopeModel.joinConditions.splice(index, 1);
                    }
                };

                $scope.scopeModel.validateGrid = function () {
                    if ($scope.scopeModel.joinConditions.length == 0) {
                        return 'At least one condition should be added';
                    }
                };

                UtilsService.waitMultiplePromises([dataRecordStorageReadyDeferred.promise, joinTypeSelectorReadyDeferred.promise, joinConditionsGridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    joinTypeSelectorAPI.clearDataSource();

                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        joinSettings = payload.joinSettings;

                        if (joinSettings != undefined) {
                            selectedRecordStorageSelectionChangeDeffered = UtilsService.createPromiseDeferred();
                            recordStorageId = joinSettings.RecordStorageId;
                            joinType = joinSettings.JoinType;
                            joinConditions = joinSettings.JoinConditions;
                        }
                    }

                    initialPromises.push(loadDataRecordStorage());
                    loadJoinTypeSelector();


                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var dataItemsLoadPromises = [];

                            if (joinConditions.length > 0) {
                                $scope.scopeModel.isGridLoading = true;
                                for (var i = 0; i < joinConditions.length; i++) {
                                    var dataItem = {
                                        payload: joinConditions[i]
                                    };

                                    addItemToJoinConditionGrid(dataItem, dataItemsLoadPromises);
                                }
                               
                            }
                            return {
                                promises: dataItemsLoadPromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        $scope.scopeModel.isGridLoading = false;
                        selectedRecordStorageSelectionChangeDeffered = undefined;

                        var joinCondtionsList = $scope.scopeModel.joinConditions;
                        var joinCondtionsListLength = joinCondtionsList.length;

                        for (var i = 0; i < joinCondtionsListLength; i++) {
                            var dataItem = joinCondtionsList[i];
                            dataItem.sourceStorageJoinNameSelectionChangeDeffered = undefined;
                        }
                    });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.Joins.RDBDataRecordStorageJoinOtherRecordStorage, Vanrise.GenericData.RDBDataStorage",
                        RecordStorageId: dataRecordStorageAPI.getSelectedIds(),
                        JoinType: $scope.scopeModel.selectedJoinType.value,
                        JoinConditions: getJoinsConditions(),
                        StorageFieldEditor: "vr-genericadata-rdbdatarecordstoragesettings-joinotherrecordstorage-storagefieldeditor"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadDataRecordStorage() {
                var dataRecordStorageLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordStorageReadyDeferred.promise.then(function () {
                    var dataRecordStoragePayload = {
                        filters: [{
                            $type: "Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageFilter, Vanrise.GenericData.RDBDataStorage"
                        }]
                    };

                    if (recordStorageId != undefined)
                        dataRecordStoragePayload.selectedIds = recordStorageId;

                    VRUIUtilsService.callDirectiveLoad(dataRecordStorageAPI, dataRecordStoragePayload, dataRecordStorageLoadDeferred);
                });
                return dataRecordStorageLoadDeferred.promise;
            }

            function loadJoinTypeSelector() {
                var joinTypes = UtilsService.getArrayEnum(VR_GenericData_RDBJoinTypeEnum);
                for (var i = 0; i < joinTypes.length; i++) {
                    var jointype = joinTypes[i];
                    $scope.scopeModel.joinTypes.push(jointype);
                }

                if (joinType != undefined)
                    $scope.scopeModel.selectedJoinType = UtilsService.getItemByVal(joinTypes, joinType, "value");
            }

            function addJoinCondition() {
                $scope.scopeModel.isGridLoading = true;
                addItemToJoinConditionGridFromAdd();
            }


            function addItemToJoinConditionGrid(gridItem, dataItemsLoadPromises) {
                var dataItem = {
                    id: $scope.scopeModel.joinConditions.length + 1,
                    sourceStorageJoinNames: [],
                    storageToJoinFieldNameSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                    loadStorageToJoinFieldSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),  
                    joinFieldNameDirectiveReadyDeferred: UtilsService.createPromiseDeferred(),
                    loadJoinFieldNameDirectiveReadyDeferred: UtilsService.createPromiseDeferred()
                };

                dataItem.onJoinFieldNameDirectiveReady = function (api) {
                    dataItem.joinFieldNameDirectiveAPI = api;
                    dataItem.joinFieldNameDirectiveReadyDeferred.resolve();
                };

                dataItem.joinFieldNameDirectiveReadyDeferred.promise.then(function () {
                    var joinFieldNameDirectivePayload = {
                        context: context,
                        joinName: gridItem.payload.SourceStorageJoinName,
                        fieldName: gridItem.payload.SourceStorageFieldName,
                        loadMainFieldSelector: true
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.joinFieldNameDirectiveAPI, joinFieldNameDirectivePayload, dataItem.loadJoinFieldNameDirectiveReadyDeferred);
                });

                dataItem.onStorageToJoinFieldNameSelectorReady = function (api) {
                    dataItem.storageToJoinFieldNameSelctorAPI = api;
                    dataItem.storageToJoinFieldNameSelectorReadyDeferred.resolve();
                };

                dataItem.storageToJoinFieldNameSelectorReadyDeferred.promise.then(function () {
                    VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(recordStorageId).then(function (response) {
                        if (response != undefined) {
                            var storageToJoinDataRecordTypeFieldSelectorPayload = {
                                dataRecordTypeId: response.DataRecordTypeId,
                                selectedIds: gridItem.payload.StorageToJoinFieldName
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataItem.storageToJoinFieldNameSelctorAPI, storageToJoinDataRecordTypeFieldSelectorPayload, dataItem.loadStorageToJoinFieldSelectorPromiseDeferred);
                    });
                });
                
                dataItemsLoadPromises.push(dataItem.loadJoinFieldNameDirectiveReadyDeferred.promise);
                dataItemsLoadPromises.push(dataItem.loadStorageToJoinFieldSelectorPromiseDeferred.promise);

                $scope.scopeModel.joinConditions.push(dataItem);
            }

            function addItemToJoinConditionGridFromAdd() {
                var dataItem = {
                    id: $scope.scopeModel.joinConditions.length + 1,
                    sourceFieldNameReadyDeferred: UtilsService.createPromiseDeferred(),
                    sourceStorageJoinNames: [],
                };

                dataItem.onJoinFieldNameDirectiveReady = function (api) {
                    dataItem.joinFieldNameDirectiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isGridLoading = value;
                    };
                    var joinFieldNameDirectivePayload = {
                        context: context,
                        loadMainFieldSelector: true
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.joinFieldNameDirectiveAPI, joinFieldNameDirectivePayload, setLoader);
                };

                dataItem.onStorageToJoinFieldNameSelectorReady = function (api) {
                    dataItem.storageToJoinFieldNameSelctorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isGridLoading = value;
                    };
                    var storageToJoinDataRecordTypeFieldSelectorPayload = {
                        dataRecordTypeId: currentDataRecordStorageEntity.DataRecordTypeId,
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.storageToJoinFieldNameSelctorAPI, storageToJoinDataRecordTypeFieldSelectorPayload, setLoader);
                };

                $scope.scopeModel.isGridLoading = false;
                $scope.scopeModel.joinConditions.push(dataItem);
            }

            function getJoinsConditions() {
                var columns = [];
                for (var i = 0; i < $scope.scopeModel.joinConditions.length; i++) {
                    var column = $scope.scopeModel.joinConditions[i];
                    var data = column.joinFieldNameDirectiveAPI.getData();
                    columns.push({
                        SourceStorageJoinName: data.JoinName,
                        SourceStorageFieldName: data.FieldName,
                        StorageToJoinFieldName: column.storageToJoinFieldNameSelctorAPI.getSelectedIds()
                    });
                }
                return columns;
            }
        }

        return directiveDefinitionObject;
    }]);