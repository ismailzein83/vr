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

            var VRObjectTypeDefinitionsInfo;

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            var drillDownManager;
            var objectVariables;
            $scope.scopeModel = {};
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
                    if (payload != undefined) {
                        $scope.scopeModel.query = payload.Query.ExpressionString;
                        $scope.scopeModel.connectionString = payload.ConnectionString;
                        objectVariables = payload.Objects;
                    }

                    var promises = [];

                    var loadObjectDirectivePromise = loadObjectDirective();
                    promises.push(loadObjectDirectivePromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BEBridge.Business.DatabaseSynchronizer, Vanrise.BEBridge.Business",
                        Name: "Database Synchronizer",
                        Query: { ExpressionString: $scope.scopeModel.query },
                        ConnectionString:  $scope.scopeModel.connectionString,
                        Objects: gridAPI.getData()
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
        }

        return directiveDefinitionObject;
    }]);