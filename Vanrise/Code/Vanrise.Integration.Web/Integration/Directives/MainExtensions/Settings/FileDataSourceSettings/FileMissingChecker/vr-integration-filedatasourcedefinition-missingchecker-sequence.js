"use strict";

app.directive("vrIntegrationFiledatasourcedefinitionMissingcheckerSequence", ["UtilsService", "VR_Common_PatternPartsEnum", "VRUIUtilsService",
    function (UtilsService, VR_Common_PatternPartsEnum, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SequenceCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "sequenceCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/MainExtensions/Settings/FileDataSourceSettings/FileMissingChecker/Templates/SequenceFileMissingCheckerTemplate.html"
        };

        function SequenceCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var textPatternBuilderDirectiveAPI;
            var textPatternBuilderDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.resetSequenceNumber = 0;
                $scope.scopeModel.fileImportTimeInterval = "00:15:00";

                $scope.scopeModel.onTextPatternBuilderDirectiveReady = function (api) {
                    textPatternBuilderDirectiveAPI = api;
                    textPatternBuilderDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var fileMissingChecker;

                    if (payload != undefined) {
                        fileMissingChecker = payload.fileMissingChecker;
                        if (fileMissingChecker != undefined) {
                            $scope.scopeModel.resetSequenceNumber = fileMissingChecker.ResetSequenceNumber;
                            $scope.scopeModel.fileImportTimeInterval = fileMissingChecker.FileImportTimeInterval;
                        }
                    }

                    var textPatternBuilderDirectiveLoadPromise = loadTextPatternBuilderDirective();
                    promises.push(textPatternBuilderDirectiveLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function loadTextPatternBuilderDirective() {
                        var textPatternBuilderDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        textPatternBuilderDirectiveReadyPromiseDeferred.promise.then(function () {

                            var payload = {
                                parts: UtilsService.getArrayEnum(VR_Common_PatternPartsEnum)
                            };
                            if (fileMissingChecker != undefined) {
                                payload.textPattern = fileMissingChecker.FileNamePattern;
                            }
                            VRUIUtilsService.callDirectiveLoad(textPatternBuilderDirectiveAPI, payload, textPatternBuilderDirectiveLoadPromiseDeferred);
                        });

                        return textPatternBuilderDirectiveLoadPromiseDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileMissingChecker.SequencialFileMissingChecker, Vanrise.Integration.MainExtensions",
                        ResetSequenceNumber: $scope.scopeModel.resetSequenceNumber,
                        FileImportTimeInterval: $scope.scopeModel.fileImportTimeInterval,
                        FileNamePattern: textPatternBuilderDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }
]);