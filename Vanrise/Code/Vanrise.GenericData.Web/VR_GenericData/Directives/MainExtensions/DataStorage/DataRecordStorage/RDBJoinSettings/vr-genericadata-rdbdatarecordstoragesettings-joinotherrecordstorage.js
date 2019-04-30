//'use strict';

//app.directive('vrGenericadataRdbdatarecordstoragesettingsJoinotherrecordstorage', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_RDBJoinTypeEnum', 'VR_GenericData_DataRecordStorageAPIService',
//    function (UtilsService, VRUIUtilsService, VR_GenericData_RDBJoinTypeEnum, VR_GenericData_DataRecordStorageAPIService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new JoinOtherRecordStorageCtol(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/JoinOtherRecordStorageTemplate.html"
//        };

//        function JoinOtherRecordStorageCtol(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var context;
//            var joinsList = [];
//            var joinSettings;
//            var recordStorageId;
//            var joinType;
//            var joinConditions = [];
//            var dataRecordStorageEntity;

//            var dataRecordStorageAPI;
//            var dataRecordStorageReadyDeferred = UtilsService.createPromiseDeferred();

//            var joinTypeSelectorAPI;
//            var joinTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            var joinConditionsGridAPI;
//            var joinConditionsGridReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.joinTypes = [];
//                $scope.scopeModel.joinConditions = [];
//                $scope.scopeModel.sourceStorageJoinNames = [];

//                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
//                    dataRecordStorageAPI = api;
//                    dataRecordStorageReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onJoinTypeSelectorReady = function (api) {
//                    console.log(api);
//                    joinTypeSelectorAPI = api;
//                    joinTypeSelectorReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onJoinConditionsGridReady = function (api) {
//                    joinConditionsGridAPI = api;
//                    joinConditionsGridReadyDeferred.resolve();
//                };

//                $scope.scopeModel.addJoinCondition = function () {
//                    addJoinCondition();
//                };

//                $scope.scopeModel.onDataRecordStorageSelectionChanged = function (selctedDataRecordStorage) {
//                    if (selctedDataRecordStorage != undefined) {
//                        dataRecordStorageEntity = selctedDataRecordStorage;
//                        var contextObj = buildContext();
//                        console.log(contextObj.getDataRecordTypeId());
//                    }
//                };

//                $scope.scopeModel.removeJoinConditions = function (dataItem) {
//                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.joinConditions, dataItem.sourceStorageJoinNames, 'sourceStorageJoinNames');
//                    if (index > -1) {
//                        $scope.scopeModel.joinConditions.splice(index, 1);
//                    }
//                };

//                UtilsService.waitMultiplePromises([dataRecordStorageReadyDeferred.promise, joinTypeSelectorReadyDeferred.promise, joinConditionsGridReadyDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    joinTypeSelectorAPI.clearDataSource();

//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        context = payload.context;
//                        joinsList = context.getJoinsList();
//                        joinSettings = payload.joinSettings;

//                        if (joinSettings != undefined) {
//                            recordStorageId = joinSettings.RecordStorageId;
//                            joinType = joinSettings.JoinType;
//                            joinConditions = joinSettings.JoinConditions;
//                            $scope.scopeModel.storageFieldEditor = joinSettings.StorageFieldEditor;
//                        }
//                    }

//                    initialPromises.push(loadDataRecordStorage());
//                    loadJoinTypeSelector();


//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            if (joinConditions.length > 0) {
//                                $scope.scopeModel.isGridLoading = true;
//                                for (var i = 0; i < joinConditions.length; i++) {
//                                    var dataItem = {
//                                        payload: joinConditions[i],
//                                        readySourceStorageJoinNameSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        loadSourceStorageJoinNameSelectorPromiseDeferred: UtilsService.createPromiseDeferred()
//                                    };
//                                    dataItemsLoadPromises.push(dataItem.loadSourceStorageJoinNameSelectorPromiseDeferred.promise);
//                                    addItemToJoinConditionGrid(dataItem, settings.InputArgumentsMapping);

//                                }
//                                $scope.scopeModel.isGridLoading = false;
//                            }
//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.Joins.RDBDataRecordStorageJoinOtherRecordStorage, Vanrise.GenericData.RDBDataStorage",
//                        RecordStorageId: dataRecordStorageAPI.getSelectedIds(),
//                        JoinType: $scope.scopeModel.selectedJoinType.value,
//                        //JoinConditions: 
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function loadDataRecordStorage() {
//                var dataRecordStorageLoadDeferred = UtilsService.createPromiseDeferred();

//                dataRecordStorageReadyDeferred.promise.then(function () {
//                    var dataRecordStoragePayload = {
//                        filters: [{
//                            $type: "Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageFilter, Vanrise.GenericData.RDBDataStorage"
//                        }]
//                    };

//                    if (recordStorageId != undefined)
//                        dataRecordStoragePayload.selectedIds = recordStorageId;

//                    VRUIUtilsService.callDirectiveLoad(dataRecordStorageAPI, dataRecordStoragePayload, dataRecordStorageLoadDeferred);
//                });
//                return dataRecordStorageLoadDeferred.promise;
//            }

//            function loadJoinTypeSelector() {
//                var joinTypes = UtilsService.getArrayEnum(VR_GenericData_RDBJoinTypeEnum);
//                for (var i = 0; i < joinTypes.length; i++) {
//                    var jointype = joinTypes[i];
//                    $scope.scopeModel.joinTypes.push(jointype);
//                }

//                if (joinType != undefined)
//                    $scope.scopeModel.selectedJoinType = UtilsService.getItemByVal(joinTypes, joinType, "value");
//                else
//                    $scope.scopeModel.selectedJoinType = joinTypes[0];
//            }

//            function addJoinCondition() {
//                $scope.scopeModel.isGridLoading = true;
//                addItemToJoinConditionGridFromAdd();
//            }


//            function addItemToJoinConditionGrid(dataItem) {
//                dataItem = {
//                    id: $scope.scopeModel.joinConditions.length + 1,
//                };

//                dataItem.onSourceStorageJoinNameSelectorReady = function (api) {
//                    dataItem.sourceStorageJoinNameSelectorAPI = api;
//                    dataItem.readySourceStorageJoinNameSelectorPromiseDeferred.resolve();
//                };

//                dataItem.readySourceStorageJoinNameSelectorPromiseDeferred.promise.then(function () {
//                    //    var sourceStorageJoinNameSelectorPayload = {
//                    //    };
//                    //    VRUIUtilsService.callDirectiveLoad(dataItem.sourceStorageJoinNameSelectorAPI, sourceStorageJoinNameSelectorPayload, dataItem.loadSourceStorageJoinNameSelectorPromiseDeferred);
//                    for (var i = 0; i < joinsList.length; i++) {
//                        var join = joinsList[i];
//                        $scope.scopeModel.sourceStorageJoinNames.push(join);
//                    }
//                });

//                $scope.scopeModel.joinConditions.push(dataItem);
//            }

//            function addItemToJoinConditionGridFromAdd() {
//                var dataItem = {
//                    id: $scope.scopeModel.joinConditions.length + 1,
//                    sourceStorageJoinNames: [],
//                    sourceMainFieldSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
//                    loadSourceMainFieldSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
//                    sourceFieldNameReadyDeferred: UtilsService.createPromiseDeferred()
//                };

//                dataItem.onSourceStorageJoinNameSelectorReady = function (api) {
//                    dataItem.dataRecordTypeFieldsAPI = api;

//                    for (var i = 0; i < joinsList.length; i++) {
//                        var join = joinsList[i];
//                        dataItem.sourceStorageJoinNames.push(join);
//                    }
//                };

//                dataItem.onSourceStorageJoinNameSelectionChanged = function (item) {
//                    if (item != undefined) {
//                        console.log(item);
//                        dataItem.storageFieldEditor = item.Settings.StorageFieldEditor;
//                        if (dataItem.selectedSourceStorageJoinNameSelectionChangeDeffered != undefined) {
//                            dataItem.selectedSourceStorageJoinNameSelectionChangeDeffered.resolve();
//                        }
//                        else {
//                            dataItem.sourceFieldNameReadyDeferred.promise.then(function () {
//                                var setLoader = function (value) { $scope.scopeModel.isGridLoading = value; };
//                                var directivePayload = {
//                                    context: buildContext()
//                                };
//                                if (dataItem.sourceStorageFieldName != undefined) {
//                                    directivePayload.sourceStorageFieldName = dataItem.sourceStorageFieldName;
//                                }
//                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.sourceFieldNameAPI, directivePayload, setLoader);
//                            });
//                        }
//                    }
//                };

//                dataItem.onSourceMainFieldSelectorDirectiveReady = function (api) {
//                    dataItem.sourceMainFieldSelectorAPI = api;
//                    dataItem.sourceMainFieldSelectorReadyDeferred.resolve();
//                };

//                dataItem.onSourceStorageFieldNameDirectiveReady = function (api) {
//                    dataItem.sourceFieldNameAPI = api;
//                    dataItem.sourceFieldNameReadyDeferred.resolve();
//                };

//                dataItem.sourceMainFieldSelectorReadyDeferred.promise.then(function () {
//                    var mainDataRecordTypeFieldSelectorPayload = {
//                        dataRecordTypeId: context.getMainDataRecordTypeId(),
//                        selectedIds: dataItem.sourceStorageFieldName
//                    };
//                    VRUIUtilsService.callDirectiveLoad(dataItem.sourceMainFieldSelectorAPI, mainDataRecordTypeFieldSelectorPayload, dataItem.loadSourceMainFieldSelectorPromiseDeferred);
//                });

//                $scope.scopeModel.isGridLoading = false;
//                $scope.scopeModel.joinConditions.push(dataItem);
//            }

//            function buildContext() {
//                var context = {
//                    getDataRecordTypeId: function () {
//                        if (dataRecordStorageEntity != undefined)
//                            return dataRecordStorageEntity.DataRecordTypeId;
//                        //else {
//                        //    var recordStorageId = dataRecordStorageAPI.getSelectedIds();
//                        //    getDataRecordStorageEntity(recordStorageId).then(function () {
//                        //        console.log(dataRecordStorageEntity);
//                        //        if (dataRecordStorageEntity != undefined)
//                        //            return dataRecordStorageEntity.DataRecordTypeId;
//                        //    });
//                        //}
//                    }
//                };
//                return context;
//            }

//            function getDataRecordStorageEntity(recordStorageId) {
//                if (recordStorageId != undefined) {
//                    return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(recordStorageId).then(function (response) {
//                        dataRecordStorageEntity = response;
//                    });
//                }
//            }
//        }

//        return directiveDefinitionObject;
//    }]);