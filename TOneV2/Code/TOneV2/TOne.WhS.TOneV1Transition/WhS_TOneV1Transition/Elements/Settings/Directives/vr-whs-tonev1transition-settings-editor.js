'use strict';

app.directive('vrWhsTonev1transitionSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_TOneV1Transition/Elements/Settings/Directives/Templates/TOneV1TransitionSettingsEditorTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var sourceMigrationReaderDirectiveAPI;
            var sourceMigrationReaderDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var buildRouteTaskDirectiveAPI;
            var buildRouteTaskDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSourceMigrationReaderDirectiveReady = function (api) {
                    sourceMigrationReaderDirectiveAPI = api;
                    sourceMigrationReaderDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onBuildRouteTaskDirectiveReady = function (api) {
                    buildRouteTaskDirectiveAPI = api;
                    buildRouteTaskDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var dbSyncTaskActionArgument;
                    var routingProcessInput;

                    if (payload != undefined && payload.data != undefined) {
                        dbSyncTaskActionArgument = payload.data.DBSyncTaskActionArgument;
                        routingProcessInput = payload.data.RoutingProcessInput

                        if (payload.data.Settings) {
                            $scope.scopeModel.routingMigrationOffsetInMin = payload.data.Settings.RoutingMigrationOffsetInMin;
                        }
                    }

                    //Loading Source Migration Reader Directive
                    var sourceMigrationReaderDirectiveLoadPromiseDeferred = getSourceMigrationReaderDirectiveLoadPromiseDeferred();
                    promises.push(sourceMigrationReaderDirectiveLoadPromiseDeferred.promise);

                    //Loading Build Route Task Directive
                    var buildRouteTaskDirectiveLoadPromiseDeferred = getBuildRouteTaskDirectiveLoadPromiseDeferred();
                    promises.push(buildRouteTaskDirectiveLoadPromiseDeferred.promise);


                    function getSourceMigrationReaderDirectiveLoadPromiseDeferred() {
                        var sourceMigrationReaderDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        sourceMigrationReaderDirectiveReadyPromiseDeferred.promise.then(function () {

                            var sourceMigrationReaderDirectivePayload;
                            if (dbSyncTaskActionArgument != undefined) {
                                sourceMigrationReaderDirectivePayload = {
                                    data: dbSyncTaskActionArgument
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(sourceMigrationReaderDirectiveAPI, sourceMigrationReaderDirectivePayload, sourceMigrationReaderDirectiveLoadPromiseDeferred);
                        });

                        return sourceMigrationReaderDirectiveLoadPromiseDeferred;
                    }
                    function getBuildRouteTaskDirectiveLoadPromiseDeferred() {
                        var buildRouteTaskDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        buildRouteTaskDirectiveReadyPromiseDeferred.promise.then(function () {

                            var buildRouteTaskDirectivePayload;
                            if (routingProcessInput != undefined) {
                                buildRouteTaskDirectivePayload = {
                                    data: routingProcessInput
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(buildRouteTaskDirectiveAPI, buildRouteTaskDirectivePayload, buildRouteTaskDirectiveLoadPromiseDeferred);
                        });

                        return buildRouteTaskDirectiveLoadPromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.TOneV1Transition.Entities.TOneV1TransitionSettingsData, TOne.WhS.TOneV1Transition.Entities",
                        DBSyncTaskActionArgument: sourceMigrationReaderDirectiveAPI.getData(),
                        RoutingProcessInput: buildRouteTaskDirectiveAPI.getData(),
                        Settings: {
                            RoutingMigrationOffsetInMin: $scope.scopeModel.routingMigrationOffsetInMin
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);