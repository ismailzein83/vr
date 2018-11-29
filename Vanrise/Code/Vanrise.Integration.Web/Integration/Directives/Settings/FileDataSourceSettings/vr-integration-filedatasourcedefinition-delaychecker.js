"use strict";

app.directive("vrIntegrationFiledatasourcedefinitionDelaychecker", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FileDelayCheckerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDelayCheckerTemplate.html"
        };

        function FileDelayCheckerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fileDelayChecker;
            var settings;

            var delayCheckerSettingsDirectiveAPI;
            var delayCheckerSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFileDelayCheckerSettingsDirectiveReady = function (api) {
                    delayCheckerSettingsDirectiveAPI = api;
                    delayCheckerSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        fileDelayChecker = payload.fileDelayChecker;
                        if (fileDelayChecker != undefined) {
                            $scope.scopeModel.name = fileDelayChecker.Name;
                            settings = fileDelayChecker.Settings;
                        }
                    }

                    var loadSettingsDirectivePromise = loadSettingsDirective();
                    promises.push(loadSettingsDirectivePromise);

                    function loadSettingsDirective() {
                        var delayCheckerSettingsDirectiveLoadPromise = UtilsService.createPromiseDeferred();

                        delayCheckerSettingsDirectiveReadyDeferred.promise.then(function () {
                            var payload;
                            if (settings != undefined) {
                                payload = { settings: settings };
                            }
                            VRUIUtilsService.callDirectiveLoad(delayCheckerSettingsDirectiveAPI, payload, delayCheckerSettingsDirectiveLoadPromise);
                        });

                        return delayCheckerSettingsDirectiveLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        FileDelayCheckerId: fileDelayChecker != undefined ? fileDelayChecker.FileDelayCheckerId : UtilsService.guid(),
                        Name: $scope.scopeModel.name,
                        Settings: delayCheckerSettingsDirectiveAPI != undefined ? delayCheckerSettingsDirectiveAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);