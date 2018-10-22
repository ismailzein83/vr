﻿(function (app) {
    'use strict';

    HuaweiHeaderBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function HuaweiHeaderBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new HeaderBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/HuaweiHeaderBinaryRecordParser.html"

        };

        function HeaderBinaryRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var recordParserDirectiveAPI;
            var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.onRecordParserDirective = function (api) {
                    recordParserDirectiveAPI = api;
                    recordParserDirectiveReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload.extendedSettings != undefined) {
                        $scope.headerLengthPosition = payload.extendedSettings.HeaderLengthPosition;
                        $scope.headerBytesLength = payload.extendedSettings.HeaderBytesLength;
                    }

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.RecordParser != null) {
                                recordParserDirectivePayload = {
                                    RecordParser: payload.extendedSettings.RecordParser.Settings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                        });
                        return recordParserDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers.HeaderRecordParser,Vanrise.DataParser.MainExtensions",
                        HeaderLengthPosition: $scope.headerLengthPosition,
                        HeaderBytesLength: $scope.headerBytesLength,
                        RecordParser: {
                            Settings: recordParserDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
    app.directive('vrDataparserHuaweiHeaderBinaryRecordParserSettings', HuaweiHeaderBinaryRecordParserDirective);

})(app);