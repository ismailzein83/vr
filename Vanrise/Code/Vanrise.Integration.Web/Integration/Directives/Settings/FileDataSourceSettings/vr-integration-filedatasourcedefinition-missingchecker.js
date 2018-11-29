"use strict";

app.directive("vrIntegrationFiledatasourcedefinitionMissingchecker", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FileMissingCheckerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileMissingCheckerTemplate.html"
        };

        function FileMissingCheckerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fileMissingChecker;
            var settings;

            var missingCheckerSettingsDirectiveAPI;
            var missingCheckerSettingsDirectiveReadyDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onFileMissingCheckerSettingsDirectiveReady = function (api) {
                    missingCheckerSettingsDirectiveAPI = api;
                    missingCheckerSettingsDirectiveReadyDeffered.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        fileMissingChecker = payload.fileMissingChecker;
                        if (fileMissingChecker != undefined) {
                            $scope.scopeModel.name = fileMissingChecker.Name;
                            settings = fileMissingChecker.Settings;
                        }
                    }

                    var loadSettingsDirectivePromise = loadSettingsDirective();
                    promises.push(loadSettingsDirectivePromise);

                    function loadSettingsDirective() {
                        var missingCheckerSettingsDirectiveLoadPromise = UtilsService.createPromiseDeferred();

                        missingCheckerSettingsDirectiveReadyDeffered.promise.then(function () {
                            var payload;
                            if (settings != undefined) {
                                payload = { settings: settings };
                            }
                            VRUIUtilsService.callDirectiveLoad(missingCheckerSettingsDirectiveAPI, payload, missingCheckerSettingsDirectiveLoadPromise);
                        });

                        return missingCheckerSettingsDirectiveLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        FileMissingCheckerId: fileMissingChecker != undefined ? fileMissingChecker.FileMissingCheckerId : UtilsService.guid(),
                        Name: $scope.scopeModel.name,
                        Settings: missingCheckerSettingsDirectiveAPI != undefined ? missingCheckerSettingsDirectiveAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);