'use strict';

app.directive('vrCommonDbreplicationProcess', ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRNotificationService', 'BusinessProcess_BPDefinitionArgumentStateAPIService', 'VRCommon_DBReplicationDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRValidationService, VRNotificationService, BusinessProcess_BPDefinitionArgumentStateAPIService, VRCommon_DBReplicationDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new DBReplicationProcessCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/DBReplication/DBReplicationProcessInput/Templates/DBReplicationProcessTemplate.html"
        };

        function DBReplicationProcessCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dbDefinitionEntity;

            var dbReplicationDefinitionSelectorAPI;
            var dbReplicationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onDBReplicationDefinitonSelectionChangedDeferred;

            var dbReplicationSettingsGridAPI;
            var dbReplicationSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

            var dbDefinitions;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.numberOfDaysPerInterval = 1;

                $scope.scopeModel.onDBReplicationDefinitionSelectorReady = function (api) {
                    dbReplicationDefinitionSelectorAPI = api;
                    dbReplicationDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDBReplicationDefinitionSelectionChanged = function (selectedDBReplicationDefinition) {
                    if (selectedDBReplicationDefinition != undefined) {
                        if (onDBReplicationDefinitonSelectionChangedDeferred != undefined) {
                            onDBReplicationDefinitonSelectionChangedDeferred.resolve();
                        }
                        else {
                            var filter;
                            var dbReplicationDefinitionId = selectedDBReplicationDefinition.DBReplicationDefinitionId;
                            VRCommon_DBReplicationDefinitionAPIService.GetDBDefinitionsInfo(dbReplicationDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                                dbDefinitions = response;

                                var dbReplicationSettingsGridPayload = {
                                    dbReplicationDefinitionId: dbReplicationDefinitionId,
                                    dbDefinitions: dbDefinitions
                                };

                                var setLoader = function (value) {
                                    $scope.scopeModel.isGridLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dbReplicationSettingsGridAPI, dbReplicationSettingsGridPayload, setLoader);
                            });
                        }
                    }
                };

                $scope.scopeModel.onDBReplicationSettingsGridReady = function (api) {
                    dbReplicationSettingsGridAPI = api;
                    dbReplicationSettingsGridReadyDeferred.resolve();
                };

                $scope.scopeModel.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var loadReadyDeferred = UtilsService.createPromiseDeferred();
                    if (payload != undefined && payload.bpDefinitionId != undefined) {

                        getBPDefinitionArgumentState(payload.bpDefinitionId).then(function () {
                            if (dbDefinitionEntity != undefined) {
                                onDBReplicationDefinitonSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                            }

                            loadAllControls().then(function () {
                            }).catch(function (error) {
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            }).finally(function () {
                                loadReadyDeferred.resolve();
                            });
                        });
                    }

                    function loadAllControls() {

                        return UtilsService.waitMultipleAsyncOperations([loadDBReplicationDefinitionSelector, loadStaticData, loadDBReplicationSettingsGrid]).then(function () {
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        }).finally(function () {
                        });

                        function loadDBReplicationDefinitionSelector() {
                            var dbReplicationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            dbReplicationDefinitionSelectorReadyDeferred.promise.then(function () {

                                var dbReplicationDefinitionSelectorPayload;
                                if (dbDefinitionEntity != undefined && dbDefinitionEntity.InputArgument != undefined) {
                                    dbReplicationDefinitionSelectorPayload = {
                                        dbReplicationDefinitionId: dbDefinitionEntity.InputArgument.DBReplicationDefinitionId
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(dbReplicationDefinitionSelectorAPI, dbReplicationDefinitionSelectorPayload, dbReplicationDefinitionSelectorLoadDeferred);
                            });
                            return dbReplicationDefinitionSelectorLoadDeferred.promise;
                        }

                        function loadStaticData() {

                            if (dbDefinitionEntity != undefined && dbDefinitionEntity.InputArgument != undefined) {
                                $scope.scopeModel.fromDate = dbDefinitionEntity.InputArgument.FromTime;
                                $scope.scopeModel.toDate = dbDefinitionEntity.InputArgument.ToTime;
                                $scope.scopeModel.numberOfDaysPerInterval = dbDefinitionEntity.InputArgument.NumberOfDaysPerInterval;
                            }
                        }

                        function loadDBReplicationSettingsGrid() {
                            if (dbDefinitionEntity == undefined)
                                return;

                            var dbReplicationSettingsGridLoadDeferred = UtilsService.createPromiseDeferred();
                            var filter;
                            VRCommon_DBReplicationDefinitionAPIService.GetDBDefinitionsInfo(dbDefinitionEntity.InputArgument.DBReplicationDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                                dbDefinitions = response;

                                onDBReplicationDefinitonSelectionChangedDeferred.promise.then(function () {
                                    onDBReplicationDefinitonSelectionChangedDeferred = undefined;

                                    dbReplicationSettingsGridReadyDeferred.promise.then(function () {

                                        var dbReplicationSettingsGridPayload = {
                                            dbReplicationDefinitionId: dbDefinitionEntity.InputArgument.DBReplicationDefinitionId,
                                            dbDefinitions: dbDefinitions,
                                            dbReplicationSettings: dbDefinitionEntity.InputArgument.Settings
                                        };
                                        VRUIUtilsService.callDirectiveLoad(dbReplicationSettingsGridAPI, dbReplicationSettingsGridPayload, dbReplicationSettingsGridLoadDeferred);
                                    });
                                });
                            });
                            return dbReplicationSettingsGridLoadDeferred.promise;
                        }
                    }

                    function getBPDefinitionArgumentState(bpDefinitionId) {
                        return BusinessProcess_BPDefinitionArgumentStateAPIService.GetBPDefinitionArgumentState(bpDefinitionId).then(function (response) {
                            dbDefinitionEntity = response;
                        });
                    }

                    return loadReadyDeferred.promise;
                };

                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Common.BP.Arguments.DBReplicationProcessInput,Vanrise.Common.BP.Arguments",
                            FromTime: $scope.scopeModel.fromDate,
                            ToTime: $scope.scopeModel.toDate,
                            Settings: dbReplicationSettingsGridAPI.getData(),
                            DBReplicationDefinitionId: dbReplicationDefinitionSelectorAPI.getSelectedIds(),
                            NumberOfDaysPerInterval: $scope.scopeModel.numberOfDaysPerInterval
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);