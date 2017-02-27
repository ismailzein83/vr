'use strict';

app.directive('vrBebridgeSynchronizerDatabase', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRObjectVariableService', 'VRCommon_VRObjectTypeDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRObjectVariableService, VRCommon_VRObjectTypeDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new databaseSynchronizerEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_BEBridge/Directives/MainExtensions/TargetBESynchronizer/Templates/DatabaseTargetSynchronizer.html"
        };

        function databaseSynchronizerEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var VRObjectTypeDefinitionsInfo;

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            var drillDownManager;
            var objectVariables;
            $scope.scopeModel = {};
            $scope.scopeModel.onConnectionSelectorReady = function (api) {
                connectionSelectorAPI = api;
                connectionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.objectVariables = [];

            $scope.scopeModel.onObjectDirectiveReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var vrConnectionId = payload && payload.VRConnectionId || undefined;
                    if (payload != undefined) {
                        $scope.scopeModel.insertQueryTemplate = payload.InsertQueryTemplate != undefined ? payload.InsertQueryTemplate.ExpressionString : undefined;
                        $scope.scopeModel.connectionString = payload.ConnectionString;
                        $scope.scopeModel.loggingMessageTemplate = payload.LoggingMessageTemplate != undefined ? payload.LoggingMessageTemplate.ExpressionString : undefined;
                        $scope.scopeModel.exceptionMessageTemplate = payload.ExceptionMessageTemplate != undefined ? payload.ExceptionMessageTemplate.ExpressionString : undefined;
                        objectVariables = payload.Objects;
                    }

                    var promises = [];

                    var loadObjectDirectivePromise = loadObjectDirective();
                    promises.push(loadObjectDirectivePromise);

                    var loadVRConnectionPromise = loadVRConnectionSelector(vrConnectionId);
                    promises.push(loadVRConnectionPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BEBridge.MainExtensions.Synchronizers.DatabaseSynchronizer, Vanrise.BEBridge.MainExtensions",
                        Name: "Database Synchronizer",
                        InsertQueryTemplate: { ExpressionString: $scope.scopeModel.insertQueryTemplate },
                        VRConnectionId: connectionSelectorAPI.getSelectedIds(),
                        Objects: gridAPI.getData(),
                        LoggingMessageTemplate: { ExpressionString: $scope.scopeModel.loggingMessageTemplate },
                        ExceptionMessageTemplate: { ExpressionString: $scope.scopeModel.exceptionMessageTemplate }
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadObjectDirective() {
                var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                gridReadyDeferred.promise.then(function () {
                    var objectDirectivePayload;
                    if (objectVariables != undefined) {
                        objectDirectivePayload = {
                            objects: objectVariables
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(gridAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
                });

                return objectDirectiveLoadDeferred.promise;
            }

            function loadVRConnectionSelector(vrConnectionId) {
                var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                connectionSelectorReadyDeferred.promise.then(function () {
                    var selectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Common.Business.SQLConnectionFilter ,Vanrise.Common.Business"
                            }]
                        },
                        selectedIds: vrConnectionId
                    };
                    VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
                });

                return connectionSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);